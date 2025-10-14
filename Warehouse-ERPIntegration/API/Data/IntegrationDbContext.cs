using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Warehouse_ERPIntegration.API.Models.Entity;

namespace Warehouse_ERPIntegration.API.Data
{
    public class IntegrationDbContext : DbContext
    {
        public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ExternalProductCode)
                .IsUnique();

            modelBuilder.Entity<Customer>()
               .HasIndex(c => c.ExternalCustomerId)
               .IsUnique();

            modelBuilder.Entity<PurchaseOrder>()
               .HasIndex(po => po.ExternalOrderId)
               .IsUnique();

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.Items)
                .WithOne(i => i.PurchaseOrder)
                .HasForeignKey(i => i.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesOrder>()
                 .HasIndex(so => so.ExternalOrderId)
                 .IsUnique();

            modelBuilder.Entity<SalesOrder>()
                .HasMany(so => so.Items)
                .WithOne(i => i.SalesOrder)
                .HasForeignKey(i => i.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
