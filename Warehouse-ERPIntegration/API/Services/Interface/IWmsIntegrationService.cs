using Warehouse_ERPIntegration.API.Models.DTO;

namespace Warehouse_ERPIntegration.API.Services.Interface
{
    public interface IWmsIntegrationService
    {
        Task<bool> SendPurchaseOrderAsync(PurchaseOrderDto po);
        Task<bool> SendSalesOrderAsync(SalesOrderDto so);
    }
}
