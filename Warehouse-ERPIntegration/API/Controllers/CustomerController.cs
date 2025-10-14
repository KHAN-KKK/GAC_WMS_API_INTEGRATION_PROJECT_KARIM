using Microsoft.AspNetCore.Mvc;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class CustomersController : BaseController
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerDto dto)
        {
            var result = await _service.ValidateAndCreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetByExternalId),
                new { externalId = result.Result.ExternalCustomerId },
                result.Result);
        }

        [HttpGet("{externalId}")]
        public async Task<IActionResult> GetByExternalId(string externalId)
        {
            var customer = await _service.GetByExternalIdAsync(externalId);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }
    }
}
