using System.ComponentModel.DataAnnotations;

namespace Warehouse_ERPIntegration.API.Models.Entity
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ExternalCustomerId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
