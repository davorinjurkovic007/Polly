using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace PollyDemo.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        readonly AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy;

        public CatalogController()
        {
            //httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).RetryAsync(3);
            //httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                    //.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)/2));
            httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                .RetryAsync(3, onRetry: (httpResponseMessage, retryCount) =>
                                {
                                    if(httpResponseMessage.Result.StatusCode == System.Net.HttpStatusCode.NotFound)
                                    {
                                        string something = "";
                                        // log somewhere
                                    }
                                    else if(httpResponseMessage.Result.StatusCode == System.Net.HttpStatusCode.Conflict)
                                    {
                                        // do something else
                                        string something = "";
                                    }
                                    else if(httpResponseMessage.Result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                                    {
                                        string something = "";
                                    }
                                });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            

            var httpClient = GetHttpClient();
            string requestEndpoint = $"inventory/{id}";

            //HttpResponseMessage response = await httpClient.GetAsync(requestEndpoint);
            HttpResponseMessage response = await httpRetryPolicy.ExecuteAsync(() => httpClient.GetAsync(requestEndpoint));

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
