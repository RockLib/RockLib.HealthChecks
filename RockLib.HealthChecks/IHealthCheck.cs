using System.Collections.Generic;
#if !(NET35 || NET40)
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RockLib.HealthChecks
{
    /// <summary>
    /// Defines an object that checks the health of a logical downstream dependency or sub-component of
    /// a service.
    /// </summary>
    public interface IHealthCheck
    {
        /// <summary>
        /// Gets the name of the logical downstream dependency or sub-component of a service. Must not
        /// contain a colon.
        /// </summary>
        string ComponentName { get; }

        /// <summary>
        /// Gets the name of the measurement that the status is reported for. Must not contain a colon.
        /// </summary>
        string MeasurementName { get; }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        string ComponentType { get; }

        /// <summary>
        /// Gets a unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </summary>
        string ComponentId { get; }

#if NET35 || NET40
        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <returns>A list of results with details on the component's health.</returns>
        IList<HealthCheckResult> Check();
#else
        /// <summary>
        /// Check the health of the sub-component/dependency asynchronously.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// A task list of results with details on the component's health representing the asynchronous
        /// operation.
        /// </returns>
        Task<IReadOnlyList<HealthCheckResult>> CheckAsync(CancellationToken cancellationToken = default(CancellationToken));
#endif
    }
}
