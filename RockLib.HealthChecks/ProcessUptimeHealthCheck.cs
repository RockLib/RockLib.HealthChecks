using System;
using System.Diagnostics;
#if !(NET35 || NET40)
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RockLib.HealthChecks
{
    /// <summary>
    /// A health check that records the uptime of the current process. Always passes.
    /// </summary>
    public class ProcessUptimeHealthCheck : SingleResultHealthCheck
    {
        private readonly Process _currentProcess = Process.GetCurrentProcess();

        /// <summary>
        /// Initalizes a new instance of the <see cref="ProcessUptimeHealthCheck"/> class.
        /// </summary>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Must not contain a
        /// colon.
        /// </param>
        /// <param name="measurementName">
        /// The name of the measurement that the status is reported for. Defaults to 'uptime'. Must not
        /// contain a colon.
        /// </param>
        /// <param name="componentType">The type of the component.</param>
        /// <param name="componentId">
        /// A unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </param>
        public ProcessUptimeHealthCheck(string componentName = null, string measurementName = "uptime",
            string componentType = null, string componentId = null)
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
            result.ObservedValue = (DateTime.Now - _currentProcess.StartTime).TotalSeconds;
            result.ObservedUnit = "s";
        }
    }
}
