using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.WebApi
{
    /// <summary>
    /// A message handler for running health checks.
    /// </summary>
    public sealed class HealthCheckHandler : HttpMessageHandler
    {
        private readonly IHealthCheckRunner _healthCheckRunner;
        private readonly bool _indent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckHandler"/> class.
        /// </summary>
        /// <param name="healthCheckRunner">
        /// The <see cref="IHealthCheckRunner"/> that evaluates the health of the service.
        /// </param>
        /// <param name="indent">Whether to indent the JSON output.</param>
        public HealthCheckHandler(IHealthCheckRunner healthCheckRunner, bool indent)
        {
            _healthCheckRunner = healthCheckRunner ?? throw new ArgumentNullException(nameof(healthCheckRunner));
            _indent = indent;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var healthCheckResponse = await _healthCheckRunner.RunAsync(cancellationToken).ConfigureAwait(false);

            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)healthCheckResponse.StatusCode,
                Content = new StringContent(healthCheckResponse.Serialize(_indent), Encoding.UTF8, healthCheckResponse.ContentType)
            };

            return response;
        }
    }
}
