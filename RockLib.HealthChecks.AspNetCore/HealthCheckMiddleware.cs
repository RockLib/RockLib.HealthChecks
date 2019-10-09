using Microsoft.AspNetCore.Http;
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

            _healthCheckRunner = healthCheckRunner;
            _indent = indent;
        }

        /// <summary>
        /// Invoke the middleware.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var healthCheckResponse = await _healthCheckRunner.RunAsync(context.RequestAborted).ConfigureAwait(false);

            context.Response.StatusCode = healthCheckResponse.StatusCode;
            context.Response.ContentType = healthCheckResponse.ContentType;

            await context.Response.WriteAsync(healthCheckResponse.Serialize(_indent)).ConfigureAwait(false);

            // Terminal middlewares don't invoke the 'next' delegate.
        }
    }
}
