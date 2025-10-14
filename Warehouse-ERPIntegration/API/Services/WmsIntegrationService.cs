using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Services
{
    public class WmsIntegrationService : IWmsIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _wmsBaseUrl;

        public WmsIntegrationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _wmsBaseUrl = configuration.GetValue<string>("Wms:BaseUrl"); // e.g., "https://wms.gac.com/api/"
        }

        public async Task<bool> SendPurchaseOrderAsync(PurchaseOrderDto po)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_wmsBaseUrl}purchase-orders", po);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendSalesOrderAsync(SalesOrderDto so)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_wmsBaseUrl}sales-orders", so);
            return response.IsSuccessStatusCode;
        }
    }
}
