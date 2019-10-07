using RockLib.HealthChecks;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NetFramework47Mvc.Controllers
{
    [AllowAnonymous]
    public class HealthController : Controller
    {
        public async Task<ContentResult> Index()
        {
            var healthCheckResponse = await HealthCheck.Runner.RunAsync();

            Response.StatusCode = healthCheckResponse.StatusCode;

            return Content(healthCheckResponse.Serialize(true), healthCheckResponse.ContentType);
        }
    }
}