using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Models.Entity;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IntegrationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IWmsIntegrationService _wmsService;

        public SalesOrderService(IntegrationDbContext db, IMapper mapper, IWmsIntegrationService wmsService)
        {
            _db = db;
            _mapper = mapper;
            _wmsService = wmsService;
        }

        public async Task<(bool IsSuccess, SalesOrderDto Result, IEnumerable<string> Errors)>
            ValidateAndCreateAsync(SalesOrderDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.ExternalOrderId))
                errors.Add("ExternalOrderId is required.");
            if (string.IsNullOrWhiteSpace(dto.CustomerExternalId))
                errors.Add("CustomerExternalId is required.");
            if (string.IsNullOrWhiteSpace(dto.ShipmentAddress))
                errors.Add("ShipmentAddress is required.");
            if (dto.Items == null || !dto.Items.Any(x => x.Quantity > 0))
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

            var existing = await _db.SalesOrders
                .Include(so => so.Items)
                .FirstOrDefaultAsync(so => so.ExternalOrderId == dto.ExternalOrderId);

            if (existing != null)
            {
                errors.Add("External Order Already Exists");
                return (false, _mapper.Map<SalesOrderDto>(existing), errors);
            }

            var so = new SalesOrder
            {
                ExternalOrderId = dto.ExternalOrderId,
                ProcessingDate = dto.ProcessingDate,
                CustomerId = customer.Id,
                ShipmentAddress = dto.ShipmentAddress
            };

            foreach (var item in dto.Items)
            {
                var product = await _db.products
                    .FirstOrDefaultAsync(p => p.ExternalProductCode == item.ProductCode);

                if (product == null)
                    throw new KeyNotFoundException($"Product with Id {item.ProductCode} not found");

                if (product.Quantity > 0 && product.Quantity < item.Quantity)
                {
                    errors.Add($"Insufficient stock for {product.ExternalProductCode}. Available: {product.Quantity}, Requested: {item.Quantity}");
                    continue;
                }

                product.Quantity -= item.Quantity;

                so.Items.Add(new SalesOrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity
                });
            }

            if (errors.Count > 0)
                return (false, null, errors);

            _db.SalesOrders.Add(so);
            await _db.SaveChangesAsync();
            var soDto = _mapper.Map<SalesOrderDto>(so);

            //  Sending to WMS after saving
            //UNCOMMENT THIS AFTER ADDING WMS BASE URL
            //var sent = await _wmsService.SendSalesOrderAsync(soDto);
            //if (!sent)
            //    errors.Add("Failed to send SO to WMS.");

            return (true, soDto, errors);
        }

        public async Task<SalesOrderDto> GetByExternalIdAsync(string externalId)
        {
            var so = await _db.SalesOrders
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.ExternalOrderId == externalId);

            return _mapper.Map<SalesOrderDto>(so);
        }
    }
}
