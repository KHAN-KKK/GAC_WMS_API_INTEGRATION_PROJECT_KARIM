namespace Warehouse_ERPIntegration.API.Models.DTO
{
    public class CustomerDto
    {
        public string ExternalCustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
    }
}
