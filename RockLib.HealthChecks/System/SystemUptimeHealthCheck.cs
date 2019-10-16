using System.Diagnostics;
#if !(NET35 || NET40)
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RockLib.HealthChecks.System
{
    /// <summary>
    /// A health check that records the uptime of the system. Always passes.
    /// </summary>
    public class SystemUptimeHealthCheck : SingleResultHealthCheck
    {
        private static readonly double _stopwatchFrequency = Stopwatch.Frequency;

        /// <summary>
        /// Initalizes a new instance of the <see cref="SystemUptimeHealthCheck"/> class.
        /// </summary>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Defaults to 'system'.
        /// Must not contain a colon.
        /// </param>
        /// <param name="measurementName">
        /// The name of the measurement that the status is reported for. Defaults to 'uptime'. Must not
        /// contain a colon.
        /// </param>
        /// <param name="componentType">The type of the component. Defaults to 'system'.</param>
        /// <param name="componentId">
        /// A unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </param>
        public SystemUptimeHealthCheck(string componentName = "system", string measurementName = "uptime",
            string componentType = "system", string componentId = null)
            : base(componentName, measurementName, componentType, componentId)
        {
        }

#if NET35 || NET40
        /// <inheritdoc/>
        protected override void Check(HealthCheckResult result)
        {
            SetResult(result);
        }
#else
        /// <inheritdoc/>
        protected override Task CheckAsync(HealthCheckResult result, CancellationToken cancellationToken)
        {
            SetResult(result);
            return Task.FromResult(0);
        }
#endif

        private void SetResult(HealthCheckResult result)
        {
            result.Status = HealthStatus.Pass;
            result.ObservedValue = Stopwatch.GetTimestamp() / _stopwatchFrequency;
            result.ObservedUnit = "s";
        }
    }
}
