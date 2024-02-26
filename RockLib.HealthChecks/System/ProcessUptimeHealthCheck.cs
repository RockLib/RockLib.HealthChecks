using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.System
{
    /// <summary>
    /// A health check that records the uptime of the current process. Always passes.
    /// </summary>
    public class ProcessUptimeHealthCheck : SingleResultHealthCheck
    {
        private readonly DateTime _currentProcessStartTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessUptimeHealthCheck"/> class.
        /// </summary>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Defaults to 'process'.
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
        public ProcessUptimeHealthCheck(string componentName = "process", string measurementName = "uptime",
            string componentType = "system", string? componentId = null)
            : base(componentName, measurementName, componentType, componentId)
        {
            try 
            { 
                _currentProcessStartTime = Process.GetCurrentProcess().StartTime; 
            }
            catch(Exception ex) when (ex is NotSupportedException || ex is InvalidOperationException || ex is Win32Exception) 
            { 
                _currentProcessStartTime = DateTime.Now; 
            }
        }

        /// <inheritdoc/>
        protected override Task CheckAsync(HealthCheckResult result, CancellationToken cancellationToken)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(result);
#else
        if (result is null) { throw new ArgumentNullException(nameof(result)); }
#endif

            SetResult(result);
            return Task.CompletedTask;
        }

        private void SetResult(HealthCheckResult result)
        {
            result.Status = HealthStatus.Pass;
            result.ObservedValue = (DateTime.Now - _currentProcessStartTime).TotalSeconds;
            result.ObservedUnit = "s";
        }
    }
}
