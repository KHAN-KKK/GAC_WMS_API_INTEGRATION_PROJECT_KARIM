namespace Warehouse_ERPIntegration.API.Services.Interface
{
    public interface IUnitOfWork
    {
        //just for reference created this
        // we need repository pattern for this
        CustomerService CustomerService { get; }
        ProductService ProductService { get; }
        PurchaseOrderService PurchaseOrderService { get; }
        SalesOrderService SalesOrderService { get; }

        Task<bool> Complete();
        bool HasChanges();
    }
}
