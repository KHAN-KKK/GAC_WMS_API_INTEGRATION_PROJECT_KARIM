using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Models.Entity;

namespace Warehouse_ERPIntegration.API.Controllers
{
    public class ErrorController(IntegrationDbContext db) : BaseController
    {
        [HttpGet("NotFound")]
        public ActionResult<Product> GetNotFound()
        {
            var data = db.products.Find(-1);
            if (data == null) return NotFound();
            return data;
        }

        [HttpGet("InternalError")]
        public ActionResult<Product> GetInternalError()
        {
            var data = db.products.Find(-1) ?? throw new Exception("A bad thing happened");
            return data;
        }

        [HttpGet("BadRequest")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is Bad request");
        }
    }
}
