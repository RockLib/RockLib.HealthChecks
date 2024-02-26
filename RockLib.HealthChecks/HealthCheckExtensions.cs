using System;
using System.Collections.Generic;

namespace RockLib.HealthChecks
{
    /// <summary>
    /// Provides extension methods for health checks.
    /// </summary>
    public static class HealthCheckExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="HealthCheckResult"/> with its properties set from
        /// the properties of an instance of <see cref="IHealthCheck"/>. In addition, its
        /// <see cref="HealthCheckResult.Time"/> property is also set to the current UTC time.
        /// </summary>
        /// <param name="healthCheck">An instance of <see cref="IHealthCheck"/>.</param>
        /// <returns>An instance of <see cref="HealthCheckResult"/>.</returns>
        public static HealthCheckResult CreateHealthCheckResult(this IHealthCheck healthCheck)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(healthCheck);
#else
        if (healthCheck is null) { throw new ArgumentNullException(nameof(healthCheck)); }
#endif

            var result = new HealthCheckResult
            {
                ComponentName = healthCheck.ComponentName,
                MeasurementName = healthCheck.MeasurementName,
                Time = DateTime.UtcNow
            };

            if (healthCheck.ComponentId is not null)
            {
                result.ComponentId = healthCheck.ComponentId;
            }

            if (healthCheck.ComponentType is not null)
            {
                result.ComponentType = healthCheck.ComponentType;
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of <see cref="HealthResponse"/> with its properties set from
        /// the properties of an instance of <see cref="IHealthCheckRunner"/> and a collection
        /// of <see cref="HealthCheckResult"/> values.
        /// </summary>
        /// <param name="runner">An instance of <see cref="IHealthCheckRunner"/>.</param>
        /// <param name="results">A collection of <see cref="HealthCheckResult"/> values.</param>
        /// <returns>An instance of <see cref="HealthResponse"/>.</returns>
        public static HealthResponse CreateHealthResponse(this IHealthCheckRunner runner, IEnumerable<HealthCheckResult> results)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(runner);
#else
        if (runner is null) { throw new ArgumentNullException(nameof(runner)); }
#endif
            
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(results);
#else
        if (results is null) { throw new ArgumentNullException(nameof(results)); }
#endif

            return new HealthResponse(results)
            {
                Description = runner.Description,
                ServiceId = runner.ServiceId,
                Version = runner.Version,
                ReleaseId = runner.ReleaseId,
                ContentType = runner.ContentType
            }.SetStatusCode(runner);
        }

        /// <summary>
        /// Sets the <see cref="HealthResponse.StatusCode"/> property of the <paramref name="response"/>
        /// according to the value of its <see cref="HealthResponse.Status"/> property and the values of
        /// the <see cref="IHealthCheckRunner.PassStatusCode"/>, <see cref="IHealthCheckRunner.WarnStatusCode"/>,
        /// and <see cref="IHealthCheckRunner.FailStatusCode"/> properties of the <paramref name="runner"/>.
        /// </summary>
        /// <param name="response">The <see cref="HealthResponse"/>.</param>
        /// <param name="runner">The <see cref="IHealthCheckRunner"/>.</param>
        /// <returns>The same <see cref="HealthResponse"/> object.</returns>
        public static HealthResponse SetStatusCode(this HealthResponse response, IHealthCheckRunner runner)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(response);
#else
        if (response is null) { throw new ArgumentNullException(nameof(response)); }
#endif
            
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(runner);
#else
        if (runner is null) { throw new ArgumentNullException(nameof(runner)); }
#endif

            response.StatusCode = response.Status switch {
                HealthStatus.Pass => runner.PassStatusCode,
                HealthStatus.Warn => runner.WarnStatusCode,
                HealthStatus.Fail => runner.FailStatusCode,
                _ => response.StatusCode
            };

            return response;
        }
    }
}
