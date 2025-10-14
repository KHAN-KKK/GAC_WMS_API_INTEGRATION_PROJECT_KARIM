using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse_ERPIntegration.API.Models.Entity
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ExternalOrderId { get; set; }

        [Required]
        public DateTime ProcessingDate { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
}
