using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Warehouse_ERPIntegration.API.Errors;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Services.Interface;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class ProductController(IProductService _service) : BaseController
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            var result = await _service.ValidateAndCreateAsync(dto);
            ResponseStatus status = new ResponseStatus();
            if (result.Errors.Count() > 0 || !result.IsSuccess)
            {
                status.Status = "Bad Request";
                status.StatusCode = result.statusCode;
                status.StatusMessage = result.Errors.Count() > 0 ? result.Errors.ToList() : result.Errors.Append("Product details not saved").ToList();
                status.data = new object();
                status.Count = 0;
                return BadRequest(status);
            }

            status.Status = "Success";
            status.StatusCode = result.statusCode;
            status.StatusMessage = result.Errors.Append("Product created Successfully").ToList();
            status.data = new object();
            status.Count = 1;

            return Ok(status);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            ResponseStatus status = new ResponseStatus();
            var product = await _service.GetExternalCodeAsync(code);
            if (product == null)
            {
                status.Status = "Bad Request";
                status.StatusCode = "404";
                status.StatusMessage?.Append("Product details not saved").ToList();
                status.data = new object();
                status.Count = 0;
                return NotFound(status);
            }
            status.Status = "Success";
            status.StatusCode = "200" ;
            status.StatusMessage?.Append("Get Product Successfully").ToList();
            status.data = new object();
            status.Count = 1;
            return Ok(status);
        }
    }
}
