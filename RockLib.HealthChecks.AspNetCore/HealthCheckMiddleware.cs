using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.AspNetCore
{
    /// <summary>
    /// A terminal middleware for running health checks.
    /// </summary>
    public sealed class HealthCheckMiddleware
    {
        private readonly IHealthCheckRunner _healthCheckRunner;
        private readonly bool _indent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// Ignored. Required to exist in the constructor in order to meet the definition of a middleware.
        /// </param>
        /// <param name="healthCheckRunners">
        /// A collection of <see cref="IHealthCheckRunner"/> objects. The one whose <see cref="IHealthCheckRunner.Name"/>
        /// matches the <paramref name="healthCheckRunnerName"/> parameter is the one used by this middleware.
        /// </param>
        /// <param name="healthCheckRunnerName">The name of the health check runner to use.</param>
        /// <param name="indent">Whether to indent the JSON output.</param>
        /// <remarks>
        /// This constructor exists primarily so that the <see cref="HealthCheckMiddleware"/> class "plays nice" with
        /// dependency injection.
        /// </remarks>
        public HealthCheckMiddleware(RequestDelegate next, IEnumerable<IHealthCheckRunner> healthCheckRunners, string healthCheckRunnerName, bool indent = false)
            : this(next, GetHealthCheckRunner(healthCheckRunners?.ToList() ?? throw new ArgumentNullException(nameof(healthCheckRunners)), healthCheckRunnerName), indent)
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
        /// <param name="indent">Whether to indent the JSON output.</param>
        public HealthCheckMiddleware(RequestDelegate next, IHealthCheckRunner healthCheckRunner, bool indent)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // This is a terminal middleware, so throw away the RequestDelegate.

            _healthCheckRunner = healthCheckRunner ?? HealthCheck.GetRunner();
            _indent = indent;
        }

        /// <summary>
        /// Invoke the middleware.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var healthResponse = await _healthCheckRunner.RunAsync(context.RequestAborted).ConfigureAwait(false);

            context.Response.StatusCode = healthResponse.StatusCode;
            context.Response.ContentType = healthResponse.ContentType;

            await context.Response.WriteAsync(healthResponse.Serialize(_indent)).ConfigureAwait(false);

            // Terminal middlewares don't invoke the 'next' delegate.
        }

        private static IHealthCheckRunner GetHealthCheckRunner(IEnumerable<IHealthCheckRunner> healthCheckRunners, string healthCheckRunnerName)
        {
            var runners = healthCheckRunners.ToList();

            if (runners.Count == 0)
                return HealthCheck.GetRunner(healthCheckRunnerName);

            healthCheckRunnerName = GetName(healthCheckRunnerName);
            return runners.SingleOrDefault(runner => healthCheckRunnerName.Equals(GetName(runner.Name)))
                ?? throw new InvalidOperationException($"One or more health check runners were provided, but none match the specified name, '{healthCheckRunnerName}'.");
        }

        private static string GetName(string name) => string.IsNullOrEmpty(name) ? "default" : name;
    }
}
