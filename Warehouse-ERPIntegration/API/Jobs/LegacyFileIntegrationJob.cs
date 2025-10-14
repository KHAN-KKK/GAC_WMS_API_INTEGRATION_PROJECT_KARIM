using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Xml.Linq;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Models.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Warehouse_ERPIntegration.API.Jobs
{
    [DisallowConcurrentExecution]
    public class LegacyFileIntegrationJob : IJob
    {
        private readonly ILogger<LegacyFileIntegrationJob> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public LegacyFileIntegrationJob(
            ILogger<LegacyFileIntegrationJob> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var folderPath = _config["LegacyIntegration:InputFolder"]!;
            var errorPath = _config["LegacyIntegration:ErrorPath"]!;

            _logger.LogInformation("Checking folder: {folder}", folderPath);

            if (!Directory.Exists(folderPath))
            {
                _logger.LogWarning("Folder not found: {folder}", folderPath);
                return;
            }

            //var allFiles = Directory.GetFiles(folderPath);
            //_logger.LogInformation("Total files in folder: {count}", allFiles.Length);
            //foreach (var f in allFiles)
            //{
            //    _logger.LogInformation("File found: {file}", f);
            //}

            //var xmlFiles = Directory.GetFiles(folderPath); //, "*.xml", SearchOption.AllDirectories);
            var xmlFiles = Directory.GetFiles(folderPath, "*.xml"); // SearchOption.AllDirectories);
            if (xmlFiles.Length == 0)
            {
                _logger.LogInformation("No XML files found at {folder}", folderPath);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();

            foreach (var file in xmlFiles)
            {
                try
                {
                    var xdoc = XDocument.Load(file);

                    try
                    {
                        var fileName = Path.GetFileName(file).ToLower();

                        if (fileName.StartsWith("products"))
                            await ProcessProductsFile(db, file);
                        else if (fileName.StartsWith("purchaseorders"))
                            await ProcessPurchaseOrdersFile(db, file);
                        else if (fileName.StartsWith("salesorders"))
                            await ProcessSalesOrdersFile(db, file);
                        else
                            _logger.LogWarning("Skipping unrecognized file: {file}", file);

                        File.Move(file, file + ".processed", true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing file {file}", file);
                        File.Move(file, file + ".error", true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file {file}", file);
                    File.Move(file, file + ".error", true);
                }
            }
        }

        private async Task ProcessProductsFile(IntegrationDbContext db, string file)
        {
            var xdoc = XDocument.Load(file);
            var products = xdoc.Descendants("Product")
                .Select(x =>
                {
                    int.TryParse(x.Element("Quantity")?.Value, out int qty);
                    return new Product
                    {
                        ExternalProductCode = x.Element("ProductCode")?.Value ?? "",
                        Title = x.Element("Title")?.Value ?? "",
                        Description = x.Element("Description")?.Value ?? "",
                        Quantity = qty > 0 ? qty : 0
                    };
                    //Dimensions = x.Element("Dimensions")?.Value ?? ""
                }).ToList();

            foreach (var p in products)
            {
                if (string.IsNullOrWhiteSpace(p.ExternalProductCode))
                {
                    _logger.LogWarning("Invalid product in {file}: missing ProductCode", file);
                    return;
                }
                if (p.Quantity <= 0)
                {
                    _logger.LogWarning("Invalid quantity for Product {code} in {file}", p.ExternalProductCode, file);
                    return;
                }

                var existing = await db.products.FirstOrDefaultAsync(x => x.ExternalProductCode == p.ExternalProductCode);
                if (existing == null)
                {
                    File.Move(file, file + ".error", true);
                    return;
                }
                else
                {
                    existing.Title = p.Title;
                    existing.Description = p.Description;
                    existing.Quantity = p.Quantity;
                }
            }
            await db.SaveChangesAsync();
            _logger.LogInformation(" Processed Products file: {file}", file);
        //after this need to call the api's for products, po, so
        //https://localhost:5001/api/products
        }

        private async Task ProcessPurchaseOrdersFile(IntegrationDbContext db, string file)
        {
            var xdoc = XDocument.Load(file);

            foreach (var poElem in xdoc.Descendants("PurchaseOrder"))
            {
                var ExternalorderId = poElem.Element("ExternalOrderId")?.Value ?? null;
                var existingPo = await db.PurchaseOrders
                    .Include(p => p.Items)
                    .FirstOrDefaultAsync(x => x.ExternalOrderId == ExternalorderId);

                if (existingPo != null)
                {
                    _logger.LogInformation("Skipping existing PO: {orderId}", ExternalorderId);
                    File.Move(file, file + ".error", true);
                    continue;
                }

                var externalCustomerId = poElem.Element("ExternalCustomerId")?.Value;
                var customer = await db.Customers.FirstOrDefaultAsync(c => c.ExternalCustomerId == externalCustomerId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer not found: {customerCode} in {file}", externalCustomerId, file);
                    File.Move(file, file + ".error", true);
                    return;
                }


                var po = new PurchaseOrder
                {
                    ExternalOrderId = ExternalorderId,
                    ProcessingDate = DateTime.Parse(poElem.Element("ProcessingDate")?.Value ?? DateTime.Now.ToString()),
                    CustomerId = customer.Id
                };

                foreach (var itemElem in poElem.Descendants("Item"))
                {
                    var prodCode = itemElem.Element("ExternalProductCode")?.Value ?? "";
                    var product = await db.products.FirstOrDefaultAsync(p => p.ExternalProductCode == prodCode);
                    if (product == null)
                    {
                        _logger.LogInformation("Product not found {prodCode}", prodCode);
                        File.Move(file, file + ".error", true);
                        return;
                    }
                    var qty = int.Parse(itemElem.Element("Quantity").Value);
                    if (qty < 0)
                    {
                        _logger.LogInformation("Quantity must be more than zero {qty}", qty);
                        File.Move(file, file + ".error", true);
                        return;
                    }
                    
                    product.Quantity += qty;
                    po.Items.Add(new PurchaseOrderItem
                    {
                        ProductId = product.Id,
                        Quantity = qty,
                    });
                }

                db.PurchaseOrders.Add(po);
                await db.SaveChangesAsync();
                _logger.LogInformation(" Processed Purchase Order: {orderId}", ExternalorderId);

                //after this need to call the api's for products, po, so
            }
        }

        private async Task ProcessSalesOrdersFile(IntegrationDbContext db, string file)
        {
            var xdoc = XDocument.Load(file);

            foreach (var soElem in xdoc.Descendants("SalesOrder"))
            {
                var orderId = soElem.Element("OrderId")?.Value ?? "";
                var existingSo = await db.SalesOrders
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(x => x.ExternalOrderId == orderId);

                if (existingSo != null)
                {
                    _logger.LogInformation("Skipping existing SO: {orderId}", orderId);
                    File.Move(file, file + ".error", true);
                    return;
                }

                var customerCode = soElem.Element("ExternalCustomerId")?.Value ?? "";
                var customer = await db.Customers.FirstOrDefaultAsync(c => c.ExternalCustomerId == customerCode);

                if (customer == null)
                {
                    _logger.LogInformation("Customer Not Found {customerCode}", customerCode);
                    File.Move(file, file + ".error", true);
                    return;
                }

                var so = new SalesOrder
                {
                    ExternalOrderId = orderId,
                    ProcessingDate = DateTime.Parse(soElem.Element("ProcessingDate")?.Value ?? DateTime.Now.ToString()),
                    CustomerId = customer.Id,
                    ShipmentAddress = soElem.Element("ShipmentAddress")?.Value ?? ""
                };

                foreach (var itemElem in soElem.Descendants("Item"))
                {
                    var prodCode = itemElem.Element("ExternalProductCode")?.Value ?? "";
                    var product = await db.products.FirstOrDefaultAsync(p => p.ExternalProductCode == prodCode);
                    if (product == null)
                    {
                        _logger.LogInformation("Product Not Found {prodCode}", prodCode);
                        File.Move(file, file + ".error", true);
                        return;
                    }
                    var qty = int.Parse(itemElem.Element("Quantity").Value);
                    if (qty < 0)
                    {
                        _logger.LogInformation("Quantity must be more than zero {qty}", qty);
                        File.Move(file, file + ".error", true);
                        return;
                    }
                    if (product.Quantity < qty)
                    {
                        _logger.LogInformation($"Insufficient stock for {product.ExternalProductCode}. Available: {product.Quantity}, Requested: {qty}");
                        File.Move(file, file + ".error", true);
                        return;
                    }
                    product.Quantity -= qty;
                    so.Items.Add(new SalesOrderItem
                    {
                        ProductId = product.Id,
                        Quantity = int.Parse(itemElem.Element("Quantity")?.Value ?? "0")
                    });
                }

                db.SalesOrders.Add(so);
                await db.SaveChangesAsync();
                _logger.LogInformation(" Processed Sales Order: {orderId}", orderId);
                //after this need to call the api's for products, po, so
            }
        }
    }
}
