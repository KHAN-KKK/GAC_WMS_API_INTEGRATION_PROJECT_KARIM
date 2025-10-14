using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Services;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class SalesOrdersController : BaseController
    {
        private readonly ISalesOrderService _service;
        private readonly IWmsIntegrationService _wmsService;

        public SalesOrdersController(ISalesOrderService service, IWmsIntegrationService wmsService)
        {
            _service = service;
            _wmsService = wmsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalesOrderDto dto)
        {
            var result = await _service.ValidateAndCreateAsync(dto);

            if (result.Errors.Any() || !result.IsSuccess)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetByExternalId),
                new { externalId = result.Result.ExternalOrderId },
                result.Result);
        }

        [HttpGet("{externalId}")]
        public async Task<IActionResult> GetByExternalId(string externalId)
        {
            var order = await _service.GetByExternalIdAsync(externalId);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        //should call either from outside like micro services
        //or admin should call this if the po failed due to some reason
        [HttpPost("send-to-wms/{externalId}")]
        public async Task<IActionResult> SendToWms(string externalId)
        {
            var so = await _service.GetByExternalIdAsync(externalId);
            if (so == null) return NotFound();

            var success = await _wmsService.SendSalesOrderAsync(so);
            if (!success) return StatusCode(500, "Failed to send to WMS");

            return Ok("SO sent to WMS successfully");
        }
    }
}
