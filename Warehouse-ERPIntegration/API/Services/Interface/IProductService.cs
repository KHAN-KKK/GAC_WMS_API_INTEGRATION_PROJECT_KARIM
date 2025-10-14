using Warehouse_ERPIntegration.API.Models.DTO;

namespace Warehouse_ERPIntegration.API.Services.Interface
{
    public interface IProductService 
    {
        Task<(bool IsSuccess, ProductDto Result, IEnumerable<string> Errors)> ValidateAndCreateAsync(ProductDto dto);
        Task<ProductDto> GetExternalCodeAsync(string code);
    }
}
