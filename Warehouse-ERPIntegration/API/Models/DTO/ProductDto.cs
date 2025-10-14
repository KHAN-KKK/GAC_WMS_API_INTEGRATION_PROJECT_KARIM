namespace Warehouse_ERPIntegration.API.Models.DTO
{
    public class ProductDto
    {
        public string ExternalProductCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        //public double? Weight { get; set; }
        //public double? Length { get; set; }
        //public double? Width { get; set; }
        //public double? Height { get; set; }
    }
}
