namespace Warehouse_ERPIntegration.API.Middleware
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
