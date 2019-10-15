using RockLib.HealthChecks;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mvc.net47
{
    [AllowAnonymous]
    public class HealthController : Controller
    {
        public async Task<ContentResult> Index()
        {
            var healthCheckResponse = await HealthCheck.GetRunner().RunAsync();

            Response.StatusCode = healthCheckResponse.StatusCode;

            return Content(healthCheckResponse.Serialize(true), healthCheckResponse.ContentType);
        }
    }
}