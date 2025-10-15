using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Warehouse_ERPIntegration.API.Errors;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Services;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class PurchaseOrdersController(IPurchaseOrderService _service, IWmsIntegrationService _wmsService) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderDto dto)
        {
            var result = await _service.ValidateAndCreateAsync(dto);

            ResponseStatus status = new ResponseStatus();
            if (result.Errors.Count() > 0 || !result.IsSuccess)
            {
                status.Status = "Bad Request";
                status.StatusCode = "400";
                status.StatusMessage = result.Errors.Count() > 0 ? result.Errors.ToList() : result.Errors.Append("Product details not saved").ToList();
                status.data = new object();
                status.Count = 0;
                return BadRequest(status);
            }

            status.Status = "Success";
            status.StatusCode = "201";
            status.StatusMessage = result.Errors.Append("Product Orders created Successfully").ToList();
            status.data = new object();
            status.Count = 1;

            return Ok(status);
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
            var po = await _service.GetByExternalIdAsync(externalId);
            if (po == null) return NotFound();

            var success = await _wmsService.SendPurchaseOrderAsync(po);
            if (!success) return StatusCode(500, "Failed to send to WMS");

            return Ok("PO sent to WMS successfully");
        }
    }
}
