using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace RockLib.HealthChecks.AspNetCore
{
    /// <summary>
    /// Extensions for using RockLib.HealthChecks with ASP.NET Core middleware.
    /// </summary>
    public static class HealthCheckMiddlewareExtensions
    {
        /// <summary>
        /// Adds a terminal <see cref="HealthCheckMiddleware"/> to the application.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="healthCheckRunnerName">
        /// The name of the health runner that will perform health checks for the health endpoint.
        /// </param>
        /// <param name="route">The route of the health endpoint.</param>
        /// <param name="indent">Whether to indent the JSON output.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseRockLibHealthChecks(this IApplicationBuilder builder,
            string healthCheckRunnerName = "", string route = "/health", bool indent = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (healthCheckRunnerName == null)
                throw new ArgumentNullException(nameof(healthCheckRunnerName));
            if (route == null)
                throw new ArgumentNullException(nameof(route));

            var path = new PathString($"/{route.Trim('/')}");

            return builder.Map(path, appBuilder =>
                appBuilder.UseMiddleware<HealthCheckMiddleware>(healthCheckRunnerName, indent));
        }
    }
}
