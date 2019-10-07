using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.AspNetCore
{
    internal sealed class HealthCheckMiddleware
    {
        private readonly IHealthCheckRunner _healthCheckRunner;
        private readonly bool _indent;

        public HealthCheckMiddleware(RequestDelegate next, IHealthCheckRunner healthCheckRunner, bool indent)
        {
            // Throw away the next delegate...
            _healthCheckRunner = healthCheckRunner;
            _indent = indent;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var healthCheckResponse = await _healthCheckRunner.RunAsync(context.RequestAborted).ConfigureAwait(false);

            // Clear any content from previous middlewares.
            context.Response.Clear();
            context.Response.StatusCode = healthCheckResponse.StatusCode;
            context.Response.ContentType = healthCheckResponse.ContentType;
            await context.Response.WriteAsync(healthCheckResponse.Serialize(_indent)).ConfigureAwait(false);

            return; // short circuiting here instead of calling _next(context) to continue down the pipeline
        }
    }
}
