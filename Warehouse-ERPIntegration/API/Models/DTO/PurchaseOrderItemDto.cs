namespace Warehouse_ERPIntegration.API.Models.DTO
{
    public class PurchaseOrderItemDto
    {
        public required string ProductCode { get; set; }
        public required int Quantity { get; set; }
    }
}
