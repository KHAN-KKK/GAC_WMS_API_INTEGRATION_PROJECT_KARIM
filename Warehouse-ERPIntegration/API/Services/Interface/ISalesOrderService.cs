using Warehouse_ERPIntegration.API.Models.DTO;

namespace Warehouse_ERPIntegration.API.Services.Interface
{
    public interface ISalesOrderService
    {
        Task<(bool IsSuccess, SalesOrderDto Result, IEnumerable<string> Errors)>
            ValidateAndCreateAsync(SalesOrderDto dto);

        Task<SalesOrderDto> GetByExternalIdAsync(string externalId);
    }
}
