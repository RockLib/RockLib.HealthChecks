using RockLib.HealthChecks;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NetFramework47Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> PerformHealthCheck()
        {
            var healthCheckResponse = await HealthCheck.Runner.RunAsync();

            var httpResponseMessage = new HttpResponseMessage((HttpStatusCode)healthCheckResponse.StatusCode)
            {
                Content = new StringContent(healthCheckResponse.Serialize(true), null, healthCheckResponse.ContentType)
            };

            return ResponseMessage(httpResponseMessage);
        }
    }
}
