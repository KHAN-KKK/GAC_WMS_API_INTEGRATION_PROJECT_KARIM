namespace Warehouse_ERPIntegration.API.Models.DTO
{
    public class SalesOrderDto
    {
        public string ExternalOrderId { get; set; }
        public DateTime ProcessingDate { get; set; }
        public string CustomerExternalId { get; set; }
        public string ShipmentAddress { get; set; }
        public List<SalesOrderItemDto> Items { get; set; } = new();
    }
}
