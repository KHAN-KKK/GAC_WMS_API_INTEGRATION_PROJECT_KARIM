using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse_ERPIntegration.API.Models.Entity
{
    public class SalesOrderItem
    {
        public int Id { get; set; }

        [ForeignKey("SalesOrder")]
        public int SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        [Required]
        public int Quantity { get; set; }
    }
}
