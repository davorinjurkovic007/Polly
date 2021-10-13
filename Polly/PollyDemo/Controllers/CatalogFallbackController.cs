using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PollyDemo.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CatalogFallbackController : ControllerBase
    {
        readonly AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy;
        readonly AsyncFallbackPolicy<HttpResponseMessage> httpRequestFallbackPolicy;

        private int cachedNumber = 0;

        public CatalogFallbackController()
        {
            httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).RetryAsync(3);

            httpRequestFallbackPolicy = 
                Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    .FallbackAsync(
                       new HttpResponseMessage(HttpStatusCode.OK)
                       {
                           Content = new ObjectContent(cachedNumber.GetType(), cachedNumber, new JsonMediaTypeFormatter())
                       });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = GetHttpClient();
            string requestEndpoint = $"inventoryfallback/{id}";

            HttpResponseMessage response = await httpRequestFallbackPolicy.ExecuteAsync(
                 () => httpRetryPolicy.ExecuteAsync(
                     () => httpClient.GetAsync(requestEndpoint)));

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(@"http://localhost:17415/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }
    }
}
