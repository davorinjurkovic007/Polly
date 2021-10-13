using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PollyDemo.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/InventoryAll")]
    public class InventoryAllController : ControllerBase
    {
        static int _requestCount = 0;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _requestCount++;

            if (_requestCount % 6 != 0)
            {
                await Task.Delay(10000); // simulate some data processing by delaying for 10 seconds
            }

            return Ok(15);
        }
    }
}
