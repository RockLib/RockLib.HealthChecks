using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.WebApi
{
    internal class HealthCheckMiddleware : DelegatingHandler
    {
        private readonly IHealthCheckRunner _healthCheckRunner;
        private readonly bool _indent;

        public HealthCheckMiddleware(IHealthCheckRunner healthCheckRunner, bool indent)
        {
            _healthCheckRunner = healthCheckRunner;
            _indent = indent;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var healthCheckResponse = await _healthCheckRunner.RunAsync().ConfigureAwait(false);

            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)healthCheckResponse.StatusCode,
                Content = new StringContent(healthCheckResponse.Serialize(_indent), Encoding.UTF8, healthCheckResponse.ContentType)
            };

            return response;
        }
    }
}
