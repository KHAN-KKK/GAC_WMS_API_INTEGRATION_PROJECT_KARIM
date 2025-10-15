using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Models.Entity;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IntegrationDbContext _db;
        private readonly IMapper _mapper;

        public CustomerService(IntegrationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, int statusCode, CustomerDto Result, IEnumerable<string> Errors)> ValidateAndCreateAsync(CustomerDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.ExternalCustomerId))
                errors.Add("ExternalCustomerId is required.");
            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Customer Name is required.");
            if (string.IsNullOrWhiteSpace(dto.Address))
                errors.Add("Address is required.");

            if (errors.Count > 0)
                return (false, 400, null, errors);

            var existing = await _db.Customers
                .FirstOrDefaultAsync(c => c.ExternalCustomerId == dto.ExternalCustomerId);

            if (existing != null)
            {
                errors.Add($"Customer with Id {dto.ExternalCustomerId} already exists");
                return (false, 401, _mapper.Map<CustomerDto>(existing), errors);
            }

            var customer = _mapper.Map<Customer>(dto);
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return (true, 200 , _mapper.Map<CustomerDto>(customer), Array.Empty<string>());
        }

        public async Task<CustomerDto> GetByExternalIdAsync(string externalId)
        {
            var customer = await _db.Customers
                .FirstOrDefaultAsync(c => c.ExternalCustomerId == externalId);
            if(customer == null) throw new KeyNotFoundException($"Customer with Id {customer} not found");
            return _mapper.Map<CustomerDto>(customer);
        }
    }
}
