using Warehouse_ERPIntegration.API.Models.DTO;

namespace Warehouse_ERPIntegration.API.Services.Interface
{
    public interface ICustomerService
    {
        Task<(bool IsSuccess, CustomerDto Result, IEnumerable<string> Errors)> ValidateAndCreateAsync(CustomerDto dto);
        Task<CustomerDto> GetByExternalIdAsync(string externalId);
    }
}
