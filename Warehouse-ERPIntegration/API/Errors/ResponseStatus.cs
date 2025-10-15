namespace Warehouse_ERPIntegration.API.Errors
{
    public class ResponseStatus
    {
        public string? StatusCode { get; set; }
        public List<string>? StatusMessage { get; set; }
        public int Count { get; set; }
        public string? Status { get; set; }
        public object? data { get; set; }
    }
}
