using Warehouse_ERPIntegration.API.Models.DTO;

namespace Warehouse_ERPIntegration.API.Services.Interface
{
    public interface IPurchaseOrderService
    {
        Task<(bool IsSuccess, PurchaseOrderDto Result, IEnumerable<string> Errors)>
           ValidateAndCreateAsync(PurchaseOrderDto dto);

        Task<PurchaseOrderDto> GetByExternalIdAsync(string externalId);
    }
}
