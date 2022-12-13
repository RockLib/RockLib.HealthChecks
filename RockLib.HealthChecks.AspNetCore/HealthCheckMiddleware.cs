using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.AspNetCore;

/// <summary>
/// A terminal middleware for running health checks.
/// </summary>
public sealed class HealthCheckMiddleware
{
    private readonly IHealthCheckRunner _healthCheckRunner;
    private readonly IResponseFormatter _formatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckMiddleware"/> class using an instance of
    /// <see cref="IServiceProvider"/> to resolve its <see cref="IHealthCheckRunner"/> dependency.
    /// </summary>
    /// <param name="next">
    /// Ignored. Required to exist in the constructor in order to meet the definition of a middleware.
    /// </param>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> that can resolve the <see cref="IHealthCheckRunner"/> dependency
    /// for this instance of <see cref="HealthCheckMiddleware"/>.
    /// </param>
    /// <param name="healthCheckRunnerName">The name of the health check runner to use.</param>
    /// <param name="formatter"></param>
    /// <remarks>
    /// This constructor exists primarily so that the <see cref="HealthCheckMiddleware"/> class "plays nice" with
    /// dependency injection.
    /// <para>
    /// Implementation details: An attempt is made to resolve a collection of <see cref="IHealthCheckRunner"/>
    /// objects from the <paramref name="serviceProvider"/> parameter initially. If none can be resolved, then
    /// an <see cref="IConfiguration"/> instance is resolved, a composite "RockLib_HealthChecks" /
    /// "RockLib.HealthChecks" section is obtained, and a collection of <see cref="IHealthCheckRunner"/>
    /// objects is created using RockLib.Configuration.ObjectFactory. The <see cref="IHealthCheckRunner"/>
    /// whose name matches the <paramref name="healthCheckRunnerName"/> parameter is selected from the
    /// collection. If none match, an <see cref="InvalidOperationException"/> is thrown.
    /// </para>
    /// </remarks>
    public HealthCheckMiddleware(RequestDelegate next, IServiceProvider serviceProvider, string healthCheckRunnerName, IResponseFormatter? formatter = null)
        : this(next, GetHealthCheckRunner(serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)), healthCheckRunnerName), formatter)
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckMiddleware"/> class.
    /// </summary>
    /// <param name="next">
    /// Ignored. Required to exist in the constructor in order to meet the definition of a middleware.
    /// </param>
    /// <param name="healthCheckRunner">
    /// The <see cref="IHealthCheckRunner"/> that evaluates the health of the service.
    /// </param>
    /// <param name="formatter"></param>
    public HealthCheckMiddleware(RequestDelegate next, IHealthCheckRunner healthCheckRunner, IResponseFormatter? formatter = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        // This is a terminal middleware, so throw away the RequestDelegate.

        _healthCheckRunner = healthCheckRunner ?? throw new ArgumentNullException(nameof(healthCheckRunner));
        _formatter = formatter ?? NewtonsoftJsonResponseFormatter.DefaultInstance;
    }

    /// <summary>
    /// Invoke the middleware.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(context);
#else
        if(context is null) { throw new ArgumentNullException(nameof(context)); }
#endif
        var healthResponse = await _healthCheckRunner.RunAsync(context.RequestAborted).ConfigureAwait(false);

        context.Response.StatusCode = healthResponse.StatusCode;
        context.Response.ContentType = healthResponse.ContentType;

        await context.Response.WriteAsync(_formatter.Format(healthResponse)).ConfigureAwait(false);

        // Terminal middlewares don't invoke the 'next' delegate.
    }

    private static IHealthCheckRunner GetHealthCheckRunner(IServiceProvider serviceProvider, string healthCheckRunnerName)
    {
        var runners = serviceProvider.GetServices<IHealthCheckRunner>().ToList();

        if (runners.Count == 0)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var section = configuration.GetCompositeSection("RockLib_HealthChecks", "RockLib.HealthChecks");
            var resolver = new Resolver(type => serviceProvider.GetService(type)!);
            runners = section.Create<List<IHealthCheckRunner>>(resolver: resolver);
        }

        healthCheckRunnerName = GetName(healthCheckRunnerName);
        return runners!.SingleOrDefault(runner => healthCheckRunnerName.Equals(GetName(runner.Name), StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"One or more health check runners were provided, but none match the specified name, '{healthCheckRunnerName}'.");
    }

    private static string GetName(string? name) => string.IsNullOrEmpty(name) ? "default" : name!;
}
