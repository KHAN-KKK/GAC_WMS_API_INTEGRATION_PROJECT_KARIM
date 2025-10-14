namespace Warehouse_ERPIntegration.API.Models.DTO
{
    public class PurchaseOrderDto
    {
        public required string ExternalOrderId { get; set; }
        public DateTime ProcessingDate { get; set; }
        public required string CustomerExternalId { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }
}
