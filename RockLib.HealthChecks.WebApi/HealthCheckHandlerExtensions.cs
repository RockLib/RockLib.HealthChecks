using System;
using System.Threading;
using System.Web.Http;

namespace RockLib.HealthChecks.WebApi;

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
    /// <param name = "healthCheckRunnerName" >
    /// The name of the <see cref="IHealthCheckRunner"/> to use. If <see langword="null"/> or not provided,
    /// the default value of the <see cref="HealthCheck.GetRunner"/> method is used.
    /// </param>
    /// <param name="route">The route of the health endpoint.</param>
    /// <param name="indent">Whether to indent the JSON output.</param>
    public static void MapHealthRoute(this HttpRouteCollection routes,
        string? healthCheckRunnerName = null, string route = "/health", bool indent = false)
        => routes.MapHealthRoute(HealthCheck.GetRunner(healthCheckRunnerName)!, route, indent);


    /// <summary>
    /// Adds a health check route to the route collection.
    /// </summary>
    /// <param name="routes">The route collection.</param>
    /// <param name="healthCheckRunner">
    /// The <see cref="IHealthCheckRunner"/> to use. If <see langword="null"/>,
    /// the value of the <see cref="HealthCheck.GetRunner"/> method is used.
    /// </param>
    /// <param name="route">The route of the health endpoint.</param>
    /// <param name="indent">Whether to indent the JSON output.</param>
    public static void MapHealthRoute(this HttpRouteCollection routes,
        IHealthCheckRunner healthCheckRunner, string route = "/health", bool indent = false)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(routes);
#else
        if (routes is null) { throw new ArgumentNullException(nameof(routes)); }
#endif

        if (string.IsNullOrWhiteSpace(route))
        {
            throw new ArgumentNullException(nameof(route));
        }

        healthCheckRunner ??= HealthCheck.GetRunner()!;
        route = route.Trim('/');

        var routeIndex = Interlocked.Increment(ref _routeIndex);
        var routeName = routeIndex < 1 ? "HealthApi" : $"HealthApi-{routeIndex}";

#pragma warning disable CA2000 // Dispose objects before losing scope
        routes.MapHttpRoute(routeName, route, null, null, new HealthCheckHandler(healthCheckRunner, indent));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
