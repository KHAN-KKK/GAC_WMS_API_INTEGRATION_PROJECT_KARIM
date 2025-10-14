namespace Warehouse_ERPIntegration.API.Models.Entity
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string ExternalOrderId { get; set; }
        public DateTime ProcessingDate { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string ShipmentAddress { get; set; }

        public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
    }
}
