using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            var result = await _service.ValidateAndCreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetByCode),
                new { code = result.Result.ExternalProductCode },
                result.Result);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var product = await _service.GetExternalCodeAsync(code);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}
