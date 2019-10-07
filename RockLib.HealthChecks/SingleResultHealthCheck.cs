using System.Collections.Generic;
using System.Threading;
#if !(NET35 || NET40)
using System.Threading.Tasks;
#endif

namespace RockLib.HealthChecks
{
    /// <summary>
    /// A base class for implementations of <see cref="IHealthCheck"/> that return only one
    /// <see cref="HealthCheckResult"/>.
    /// </summary>
    public abstract class SingleResultHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultHealthCheck"/> class.
        /// </summary>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Must not contain a
        /// colon.
        /// </param>
        /// <param name="measurementName">
        /// The name of the measurement that the status is reported for. Must not contain a colon.
        /// </param>
        /// <param name="componentType">The type of the component.</param>
        /// <param name="componentId">
        /// A unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </param>
        protected SingleResultHealthCheck(string componentName = null, string measurementName = null,
            string componentType = null, string componentId = null)
        {
            ComponentName = componentName;
            MeasurementName = measurementName;
            ComponentType = componentType;
            ComponentId = componentId;
        }

        /// <summary>
        /// Gets the name of the logical downstream dependency or sub-component of a service. Must not
        /// contain a colon.
        /// </summary>
        public string ComponentName { get; }

        /// <summary>
        /// Gets the name of the measurement that the status is reported for. Must not contain a colon.
        /// </summary>
        public string MeasurementName { get; }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        public string ComponentType { get; }

        /// <summary>
        /// Gets a unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </summary>
        public string ComponentId { get; }

#if NET35 || NET40
        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <returns>A list of health check results.</returns>
        public IList<HealthCheckResult> Check()
        {
            var result = this.CreateHealthCheckResult();
            Check(result);
            return new[] { result };
        }

        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <param name="result">
        /// An object to be modified according to the outcome of the health check.
        /// </param>
        protected abstract void Check(HealthCheckResult result);

#else
        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// A task list of health check results representing the asynchronous operation.
        /// </returns>
        public async Task<IReadOnlyList<HealthCheckResult>> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = this.CreateHealthCheckResult();
            await CheckAsync(result, cancellationToken).ConfigureAwait(false);
            return new[] { result };
        }

        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <param name="result">
        /// An object to be modified according to the outcome of the health check.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected abstract Task CheckAsync(HealthCheckResult result, CancellationToken cancellationToken);
#endif
    }
}
