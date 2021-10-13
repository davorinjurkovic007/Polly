using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PollyDemo.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/InventoryFallback")]
    public class InventoryFallbackController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await Task.Delay(100);// simulate some data processing by delaying for 100 milliseconds 

            return StatusCode((int)HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }
}
