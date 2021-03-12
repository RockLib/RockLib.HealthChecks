#if NET462 || NETSTANDARD2_0 || NET5_0
namespace RockLib.HealthChecks.DependencyInjection
{
    /// <summary>
    /// Defines the editable settings for creating an instance of <see cref="IHealthCheckRunner"/>.
    /// </summary>
    public interface IHealthCheckRunnerOptions
    {
        /// <summary>
        /// Gets or sets the human-friendly description of the service.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the service, in the application scope.
        /// </summary>
        string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the public version of the service.
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Gets or sets the "release version" or "release ID" of the service.
        /// </summary>
        string ReleaseId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHealthResponseCustomizer"/> that customizes each <see cref="HealthResponse"/>
        /// object returned by the health check runner.
        /// </summary>
        IHealthResponseCustomizer ResponseCustomizer { get; set; }

        /// <summary>
        /// Gets or sets the HTTP content type of responses created by the health check runner. Must not
        /// have a null or empty value.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Pass"/>. Must have a value in the 200-399 range.
        /// </summary>
        int PassStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Warn"/>. Must have a value in the 200-399 range.
        /// </summary>
        int WarnStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Fail"/>. Must have a value in the 400-599 range.
        /// </summary>
        int FailStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HealthStatus"/> for the <see cref="HealthCheckResult"/> that is
        /// returned because an <see cref="IHealthCheck"/> has thrown an exception.
        /// </summary>
        HealthStatus? UncaughtExceptionStatus { get; set; }
    }
}
#endif
