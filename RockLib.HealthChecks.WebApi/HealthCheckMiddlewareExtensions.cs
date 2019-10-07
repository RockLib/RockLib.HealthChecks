using System.Web.Http;

namespace RockLib.HealthChecks.WebApi
{
    public static class HealthCheckMiddlewareExtensions
    {
        public static void UseRockLibHealthChecks(this HttpConfiguration config,
            IHealthCheckRunner healthCheckRunner = null, string route = "/health",
            string routeName = "HealthApi", bool indent = false)
        {
            healthCheckRunner = healthCheckRunner ?? HealthCheck.Runner;
            route = $"/{route.Trim('/')}";
            config.Routes.MapHttpRoute(routeName, route, null, null, new HealthCheckMiddleware(healthCheckRunner, indent));
        }
    }
}
