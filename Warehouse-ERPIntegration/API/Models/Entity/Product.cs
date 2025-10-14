using System.ComponentModel.DataAnnotations;

namespace Warehouse_ERPIntegration.API.Models.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ExternalProductCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }
        //public double? Weight { get; set; }
        //public double? Length { get; set; }
        //public double? Width { get; set; }
        //public double? Height { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
