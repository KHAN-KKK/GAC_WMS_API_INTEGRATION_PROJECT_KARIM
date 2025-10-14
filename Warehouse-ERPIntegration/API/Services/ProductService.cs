using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Models.Entity;
using Warehouse_ERPIntegration.API.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Warehouse_ERPIntegration.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IntegrationDbContext _db;
        private readonly IMapper _mapper;
        public ProductService(IntegrationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        
        public async Task<ProductDto> GetExternalCodeAsync(string code)
        {
            var product = await _db.products
               .FirstOrDefaultAsync(p => p.ExternalProductCode == code);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<(bool IsSuccess, ProductDto Result, IEnumerable<string> Errors)> ValidateAndCreateAsync(ProductDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.ExternalProductCode))
                errors.Add("ExternalProductCode is required.");
            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required.");
            if (dto.Quantity <= 0)
                errors.Add("Quantity is required");

            if (errors.Count > 0)
                return (false, null, errors);

            var existing = await _db.products
                .FirstOrDefaultAsync(p => p.ExternalProductCode == dto.ExternalProductCode);

            if (existing != null)
                return (false, _mapper.Map<ProductDto>(existing), Array.Empty<string>());

            var product = _mapper.Map<Product>(dto);
            _db.products.Add(product);
            await _db.SaveChangesAsync();

            return (true, _mapper.Map<ProductDto>(product), Array.Empty<string>());
        }

    }
}
