using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockLib.HealthChecks;
using System.Threading.Tasks;

namespace NetCore2Mvc.Controllers
{
    [AllowAnonymous]
    public class HealthController : Controller
    {
        private readonly IHealthCheckRunner _healthCheckRunner;

        public HealthController(IHealthCheckRunner healthCheckRunner)
        {
            _healthCheckRunner = healthCheckRunner;
        }

        public async Task<IActionResult> Index()
        {
            var healthCheckResponse = await _healthCheckRunner.RunAsync();

            Response.StatusCode = healthCheckResponse.StatusCode;

            return Content(healthCheckResponse.Serialize(true), healthCheckResponse.ContentType);
        }
    }
}
