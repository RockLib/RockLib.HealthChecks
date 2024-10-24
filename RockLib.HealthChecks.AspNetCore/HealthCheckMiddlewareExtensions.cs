using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace RockLib.HealthChecks.AspNetCore;

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
    /// <param name="formatter">
    /// The <see cref="IResponseFormatter"/> responsible for formatting health responses for the middleware's HTTP response body.
    /// </param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseRockLibHealthChecks(this IApplicationBuilder builder,
        string healthCheckRunnerName = "", string route = "/health", IResponseFormatter? formatter = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(healthCheckRunnerName);
        ArgumentNullException.ThrowIfNull(route);
#else
        if (builder is null) { throw new ArgumentNullException(nameof(builder)); }
        if (healthCheckRunnerName is null) { throw new ArgumentNullException(nameof(healthCheckRunnerName)); }
        if (route is null) { throw new ArgumentNullException(nameof(route)); }
#endif

        var path = new PathString($"/{route.Trim('/')}");

        return builder.Map(path, appBuilder =>
            appBuilder.UseMiddleware<HealthCheckMiddleware>(healthCheckRunnerName, formatter ?? NewtonsoftJsonResponseFormatter.DefaultInstance));
    }
    
    /// <summary>
    /// Exposes a means for health check dependencies to be integrated into the application.
    /// </summary>
    /// <param name="builder">the application builder</param>
    /// <returns>The application builder.</returns>
    public static IHostApplicationBuilder ConfigureRockLibHealthChecks(this IHostApplicationBuilder builder)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(builder);
#else
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }
#endif

        var config = builder.Configuration.GetSection("RockLib.HealthChecks");
        var checks = config.GetSection("healthChecks").GetChildren().ToArray();
        foreach (var checkCfg in checks)
        {
            var typeStr = checkCfg["type"];
            if (string.IsNullOrWhiteSpace(typeStr)) continue;

            Type.GetType(typeStr)?.GetMethod("Configure")?.Invoke(null, [builder]);
            Console.WriteLine($"Configured health check: {typeStr}");
        }

        return builder;
    }
}
