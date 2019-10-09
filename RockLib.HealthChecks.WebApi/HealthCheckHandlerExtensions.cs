using System;
using System.Threading;
using System.Web.Http;

namespace RockLib.HealthChecks.WebApi
{
    /// <summary>
    /// Extensions for adding a RockLib.HealthChecks route to a route collection.
    /// </summary>
    public static class HealthCheckHandlerExtensions
    {
        private static int _routeIndex = -1;

        /// <summary>
        /// Adds a health check route to the route collection.
        /// </summary>
        /// <param name="routes">The route collection.</param>
        /// <param name="healthCheckRunner">
        /// The <see cref="IHealthCheckRunner"/> to use. If <see langword="null"/> or not provided,
        /// the value of the <see cref="HealthCheck.Runner"/> property is used.
        /// </param>
        /// <param name="route">The route of the health endpoint.</param>
        /// <param name="indent">Whether to indent the JSON output.</param>
        public static void MapHealthRoute(this HttpRouteCollection routes,
            IHealthCheckRunner healthCheckRunner = null, string route = "/health", bool indent = false)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentNullException(nameof(route));

            healthCheckRunner = healthCheckRunner ?? HealthCheck.Runner;
            route = route.Trim('/');

            var routeIndex = Interlocked.Increment(ref _routeIndex);
            var routeName = routeIndex < 1 ? "HealthApi" : $"HealthApi-{routeIndex}";

            routes.MapHttpRoute(routeName, route, null, null, new HealthCheckHandler(healthCheckRunner, indent));
        }
    }
}
