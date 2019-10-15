#if !(NET35 || NET40)
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RockLib.HealthChecks
{
    /// <summary>
    /// Defines an object that checks the health of a service.
    /// </summary>
    public interface IHealthCheckRunner
    {
        /// <summary>
        /// Gets the human-friendly description of the service.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the unique identifier of the service, in the application scope.
        /// </summary>
        string ServiceId { get; }

        /// <summary>
        /// Gets the public version of the service.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the "release version" or "release ID" of the service.
        /// </summary>
        string ReleaseId { get; }

        /// <summary>
        /// Gets the HTTP content type of responses created by this health check runner. Must not
        /// have a null or empty value.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the HTTP status code of responses created by this health check runner that have a
        /// status of <see cref="HealthStatus.Pass"/>. Must have a value in the 200-399 range.
        /// </summary>
        int PassStatusCode { get; }

        /// <summary>
        /// Gets the HTTP status code of responses created by this health check runner that have a
        /// status of <see cref="HealthStatus.Warn"/>. Must have a value in the 200-399 range.
        /// </summary>
        int WarnStatusCode { get; }

        /// <summary>
        /// Gets the HTTP status code of responses created by this health check runner that have a
        /// status of <see cref="HealthStatus.Fail"/>. Must have a value in the 400-599 range.
        /// </summary>
        int FailStatusCode { get; }

#if NET35 || NET40
        /// <summary>
        /// Runs the health checks.
        /// </summary>
        /// <returns>A health response.</returns>
        HealthResponse Run();
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
        Task<HealthResponse> RunAsync(CancellationToken cancellationToken = default(CancellationToken));
#endif
    }
}