using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockLib.HealthChecks;
using System.Threading.Tasks;

namespace Mvc.netcoreapp2._2
{
    [AllowAnonymous]
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IHealthCheckRunner _healthCheckRunner;

        public HealthController(IHealthCheckRunner healthCheckRunner)
        {
            _healthCheckRunner = healthCheckRunner;
        }

        [HttpGet("")]
        public async Task<ActionResult<string>> PerformHealthCheck()
        {
            var healthCheckResponse = await _healthCheckRunner.RunAsync();

            Response.StatusCode = healthCheckResponse.StatusCode;

            return Content(healthCheckResponse.Serialize(true), healthCheckResponse.ContentType);
        }
    }
}
