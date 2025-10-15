using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Warehouse_ERPIntegration.API.Errors;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class CustomersController(ICustomerService _service) : BaseController
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerDto dto)
        {
           
            var result = await _service.ValidateAndCreateAsync(dto);
            ResponseStatus status = new ResponseStatus();
            if(result.Errors.Count() > 0 || !result.IsSuccess)
            {
                status.Status = "Bad Request";
                status.StatusCode = result.statusCode.ToString();
                status.StatusMessage = result.Errors.Count() > 0 ? result.Errors.ToList() : result.Errors.Append("Product details not saved").ToList();
                status.data = new object();
                status.Count = 0;
                return BadRequest(status);
            }

            status.Status = "Success";
            status.StatusCode = result.statusCode.ToString();
            status.StatusMessage = result.Errors.Append("Customer created Successfully").ToList();
            status.data = result.Result ;
            status.Count = 1;

            return Ok(status);
            //return CreatedAtAction(nameof(GetByExternalId),
                //new { externalId = result.Result.ExternalCustomerId },
                //result.Result);
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
