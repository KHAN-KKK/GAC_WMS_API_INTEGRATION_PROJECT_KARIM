using Microsoft.EntityFrameworkCore;
using Warehouse_ERPIntegration.API.Services;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Data
{
    
    public class UnitOfWork(IntegrationDbContext db) : IUnitOfWork
    {
        private CustomerService? _customer;
        private ProductService? _product;
        private PurchaseOrderService? _purchaseOrder;
        private SalesOrderService? _salesOrder;
        CustomerService IUnitOfWork.CustomerService => throw new NotImplementedException();

        ProductService IUnitOfWork.ProductService => throw new NotImplementedException();

        PurchaseOrderService IUnitOfWork.PurchaseOrderService => throw new NotImplementedException();

        SalesOrderService IUnitOfWork.SalesOrderService => throw new NotImplementedException();

        public async Task<bool> Complete()
        {
            try
            {
                return await db.SaveChangesAsync() > 0;
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("Error occured while saving changes" , ex);
            }
        }

        public bool HasChanges()
        {
            return db.ChangeTracker.HasChanges();
        }
    }
}
