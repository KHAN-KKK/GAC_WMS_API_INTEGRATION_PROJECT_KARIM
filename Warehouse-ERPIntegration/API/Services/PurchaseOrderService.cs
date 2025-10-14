using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Models.Entity;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IntegrationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IWmsIntegrationService _wmsService;

        public PurchaseOrderService(IntegrationDbContext db, IMapper mapper, IWmsIntegrationService wmsService)
        {
            _db = db;
            _mapper = mapper;
            _wmsService = wmsService;
        }

        public async Task<(bool IsSuccess, PurchaseOrderDto Result, IEnumerable<string> Errors)>
            ValidateAndCreateAsync(PurchaseOrderDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.ExternalOrderId))
                errors.Add("ExternalOrderId is required.");
            if (string.IsNullOrWhiteSpace(dto.CustomerExternalId))
                errors.Add("CustomerExternalId is required.");
            if (dto.Items == null || !dto.Items.Any(x=>x.Quantity > 0))
                errors.Add("At least one order item is required.");

            if (errors.Count > 0)
                return (false, null, errors);

            var customer = await _db.Customers
                .FirstOrDefaultAsync(c => c.ExternalCustomerId == dto.CustomerExternalId);

            if (customer == null)
            {
                errors.Add($"Customer {dto.CustomerExternalId} not found.");
                return (false, null, errors);
            }
            
            var existing = await _db.PurchaseOrders
                .Include(po => po.Items)
                .FirstOrDefaultAsync(po => po.ExternalOrderId == dto.ExternalOrderId);

            if (existing != null)
                    throw new KeyNotFoundException($"External Order Id {dto.ExternalOrderId} already exists");

            // Build PO with line items
            var po = new PurchaseOrder
            {
                ExternalOrderId = dto.ExternalOrderId,
                ProcessingDate = dto.ProcessingDate,
                CustomerId = customer.Id
            };

            foreach (var item in dto.Items)
            {
                var product = await _db.products
                    .FirstOrDefaultAsync(p => p.ExternalProductCode == item.ProductCode);

                if (product == null)
                  throw new KeyNotFoundException($"Product with Id {item.ProductCode} not found");

                product.Quantity += item.Quantity;

                po.Items.Add(new PurchaseOrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity
                });
            }

            if (errors.Count > 0)
                return (false, null, errors);

            _db.PurchaseOrders.Add(po);
            await _db.SaveChangesAsync();
            var poDto = _mapper.Map<PurchaseOrderDto>(po);

            //  Sending to WMS after saving
            //uncomment this after adding wms base url
            //var sent = await _wmsService.SendPurchaseOrderAsync(poDto);
            //if (!sent)
            //    errors.Add("Failed to send PO to WMS.");

            return (true, poDto, errors);
        }

        public async Task<PurchaseOrderDto> GetByExternalIdAsync(string externalId)
        {
            var po = await _db.PurchaseOrders
                .Include(p => p.Items)
                .ThenInclude(i => i.Product)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.ExternalOrderId == externalId);

            return _mapper.Map<PurchaseOrderDto>(po);
        }
    }
}
