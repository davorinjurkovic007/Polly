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
    [Route("api/InventoryReauthorization")]
    public class InventoryReauthorizationController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await Task.Delay(100); // simulate some data processing by delaying for 100 milliseconds 

            string authCode = Request.Cookies["Auth"];

            if (authCode == "GoodAuthCode")
            {
                return Ok(15);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "Not authorized");
            }

        }
    }
}
