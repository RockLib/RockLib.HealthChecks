using System;
using System.Collections.Generic;
using System.Linq;
#if NET35 || NET40
using IReadOnlyListOfIHealthCheck = System.Collections.Generic.IList<RockLib.HealthChecks.IHealthCheck>;
#else
using IReadOnlyListOfIHealthCheck = System.Collections.Generic.IReadOnlyList<RockLib.HealthChecks.IHealthCheck>;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RockLib.HealthChecks
{
    /// <summary>
    /// The default implementation of the <see cref="IHealthCheckRunner"/> interface.
    /// </summary>
    public class HealthCheckRunner : IHealthCheckRunner
    {
        internal const string DefaultContentType = "application/health+json";
        internal const int DefaultPassStatusCode = 200;
        internal const int DefaultWarnStatusCode = 200;
        internal const int DefaultFailStatusCode = 503;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckRunner"/> class.
        /// </summary>
        /// <param name="healthChecks">
        /// The collection of <see cref="IHealthCheck"/> objects to be checked by this runner. Must not have
        /// a null value.
        /// </param>
        /// <param name="description">The human-friendly description of the service.</param>
        /// <param name="serviceId">The unique identifier of the service, in the application scope.</param>
        /// <param name="version">The public version of the service.</param>
        /// <param name="releaseId">The "release version" or "release ID" of the service.</param>
        /// <param name="responseCustomizer">
        /// The <see cref="IHealthResponseCustomizer"/> that customizes each <see cref="HealthResponse"/>
        /// object returned by this runner.
        /// </param>
        /// <param name="contentType">
        /// The HTTP content type of responses created by this health check runner. Must not have a null or empty
        /// value.
        /// </param>
        /// <param name="passStatusCode">
        /// The HTTP status code of responses created by this health check runner that have a status of <see cref=
        /// "HealthStatus.Pass"/>. Must have a value in the 200-399 range.
        /// </param>
        /// <param name="warnStatusCode">
        /// The HTTP status code of responses created by this health check runner that have a status of <see cref=
        /// "HealthStatus.Warn"/>. Must have a value in the 200-399 range.
        /// </param>
        /// <param name="failStatusCode">
        /// The HTTP status code of responses created by this health check runner that have a status of <see cref=
        /// "HealthStatus.Fail"/>. Must have a value in the 400-599 range.
        /// </param>
        public HealthCheckRunner(IEnumerable<IHealthCheck> healthChecks = null,
            string description = null, string serviceId = null, string version = null, string releaseId = null,
            IHealthResponseCustomizer responseCustomizer = null, string contentType = DefaultContentType,
            int passStatusCode = DefaultPassStatusCode, int warnStatusCode = DefaultWarnStatusCode, int failStatusCode = DefaultFailStatusCode)
        {
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException(nameof(contentType));
            if (passStatusCode < 200 || passStatusCode > 399)
                throw new ArgumentOutOfRangeException(nameof(passStatusCode), "Must be in the range of 200-399.");
            if (warnStatusCode < 200 || warnStatusCode > 399)
                throw new ArgumentOutOfRangeException(nameof(warnStatusCode), "Must be in the range of 200-399.");
            if (failStatusCode < 400 || failStatusCode > 599)
                throw new ArgumentOutOfRangeException(nameof(failStatusCode), "Must be in the range of 400-599.");

            HealthChecks = (healthChecks ?? Enumerable.Empty<IHealthCheck>()) as IReadOnlyListOfIHealthCheck ?? healthChecks.ToList();
            Description = description;
            ServiceId = serviceId;
            Version = version;
            ReleaseId = releaseId;
            ResponseCustomizer = responseCustomizer;
            ContentType = contentType;
            PassStatusCode = passStatusCode;
            WarnStatusCode = warnStatusCode;
            FailStatusCode = failStatusCode;
        }

        /// <summary>
        /// Gets the list of <see cref="IHealthCheck"/> objects that are checked by this runner.
        /// </summary>
        public IReadOnlyListOfIHealthCheck HealthChecks { get; }

        /// <summary>
        /// Gets the human-friendly description of the service.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the unique identifier of the service, in the application scope.
        /// </summary>
        public string ServiceId { get; }

        /// <summary>
        /// Gets the public version of the service.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the "release version" or "release ID" of the service.
        /// </summary>
        public string ReleaseId { get; }

        /// <summary>
        /// Gets the <see cref="IHealthResponseCustomizer"/> that customizes each <see cref=
        /// "HealthResponse"/> object returned by this runner.
        /// </summary>
        public IHealthResponseCustomizer ResponseCustomizer { get; }

        /// <summary>
        /// Gets the HTTP content type of responses created by this health check runner.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the HTTP status code of responses created by this health check runner that have a
        /// status of <see cref="HealthStatus.Pass"/>.
        /// </summary>
        public int PassStatusCode { get; }

        /// <summary>
        /// Gets the HTTP status code of responses created by this health check runner that have a
        /// status of <see cref="HealthStatus.Warn"/>.
        /// </summary>
        public int WarnStatusCode { get; }

        /// <summary>
        /// Gets the HTTP status code of responses created by this health check runner that have a
        /// status of <see cref="HealthStatus.Fail"/>.
        /// </summary>
        public int FailStatusCode { get; }

#if NET35 || NET40
        /// <summary>
        /// Runs the health checks.
        /// </summary>
        /// <returns>A health response.</returns>
        public HealthResponse Run()
        {
            var healthCheckResults = HealthChecks.Select(TryCheck);
            var healthResponse = this.CreateHealthResponse(healthCheckResults.SelectMany(x => x));
            return TryCustomizeResponse(healthResponse);
        }

        private static IList<HealthCheckResult> TryCheck(IHealthCheck check)
        {
            try { return check.Check(); }
            catch (Exception ex) { return GetExceptionHealthCheckResult(check, ex); }
        }
#else
        /// <summary>
        /// Runs the health checks asynchronously.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// A task healh response representing the asynchronous operation.
        /// </returns>
        public async Task<HealthResponse> RunAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var healthCheckResultTasks = HealthChecks.Select(TryCheckAsync);
            var healthCheckResults = await Task.WhenAll(healthCheckResultTasks).ConfigureAwait(false);
            var healthResponse = this.CreateHealthResponse(healthCheckResults.SelectMany(x => x));
            return TryCustomizeResponse(healthResponse);
        }

        private static async Task<IReadOnlyList<HealthCheckResult>> TryCheckAsync(IHealthCheck check)
        {
            try { return await check.CheckAsync().ConfigureAwait(false); }
            catch (Exception ex) { return GetExceptionHealthCheckResult(check, ex); }
        }
#endif

        private HealthResponse TryCustomizeResponse(HealthResponse response)
        {
            try { ResponseCustomizer?.CustomizeResponse(response); }
            catch { }
            return response;
        }

        private static HealthCheckResult[] GetExceptionHealthCheckResult(IHealthCheck check, Exception ex)
        {
            return new[]
            {
                new HealthCheckResult
                {
                    ComponentName = check.ComponentName,
                    MeasurementName = check.MeasurementName,
                    ComponentType = check.ComponentType,
                    ComponentId = check.ComponentId,
                    Output = $"Exception in health check of type {check.GetType()}:\r\n{ex}",
                    Status = HealthStatus.Warn,
                    Time = DateTime.UtcNow
                }
            };
        }
    }
}
