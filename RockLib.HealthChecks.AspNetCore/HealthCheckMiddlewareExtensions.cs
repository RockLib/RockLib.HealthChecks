using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace RockLib.HealthChecks.AspNetCore
{
    /// <summary>
    /// Extensions for using RockLib.HealthChecks with ASP.NET Core middleware.
    /// </summary>
    public static class HealthCheckMiddlewareExtensions
    {
        /// <summary>
        /// Adds custom HealthCheck middleware to your application. Make sure this is before any authorization or authentication.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="healthCheckRunner">
        /// The <see cref="IHealthCheckRunner"/> to use. If <see langword="null"/> or not provided,
        /// the value of the <see cref="HealthCheck.Runner"/> is used.
        /// </param>
        /// <param name="route">The route of the health endpoint.</param>
        /// <param name="indent">Whether to indent the JSON output.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseRockLibHealthCheck(this IApplicationBuilder builder,
            IHealthCheckRunner healthCheckRunner = null, string route = "health", bool indent = false)
        {
            healthCheckRunner = healthCheckRunner ?? HealthCheck.Runner;
            route = $"/{route.Trim('/')}";

            return builder
                .Map(new PathString(route), appBuilder =>
                {
                    appBuilder.UseMiddleware<HealthCheckMiddleware>(healthCheckRunner, indent);
                });
        }
    }
}
