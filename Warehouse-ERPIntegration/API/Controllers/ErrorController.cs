using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Models.Entity;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class ErrorController(IntegrationDbContext db) : BaseController
    {
        [HttpGet("auth")]
        public IActionResult GetAuth()
        {
            return Unauthorized();

        }
        [HttpGet("NotFound")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("InternalError")]
        public IActionResult GetInternalError()
        {
            throw new Exception("something bad happened");
        }

        [HttpGet("BadRequest")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is Bad request");
        }
    }
}
