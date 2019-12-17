#if NET462 || NETSTANDARD2_0
using System;
using System.Collections.Generic;

namespace RockLib.HealthChecks.DependencyInjection
{
    /// <summary>
    /// Defines the settings for creating an instance of <see cref="IHealthCheckRunner"/>.
    /// </summary>
    public sealed class HealthCheckRunnerOptions : IHealthCheckRunnerOptions
    {
        private string _contentType = "application/health+json";
        private int _passStatusCode = 200;
        private int _warnStatusCode = 200;
        private int _failStatusCode = 503;

        /// <summary>
        /// Gets the health check registrations responsible for creating the runner's health checks.
        /// </summary>
        public ICollection<HealthCheckRegistration> Registrations { get; } = new List<HealthCheckRegistration>();

        /// <summary>
        /// Gets or sets the human-friendly description of the service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the service, in the application scope.
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the public version of the service.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the "release version" or "release ID" of the service.
        /// </summary>
        public string ReleaseId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHealthResponseCustomizer"/> that customizes each <see cref="HealthResponse"/>
        /// object returned by the health check runner.
        /// </summary>
        public IHealthResponseCustomizer ResponseCustomizer { get; set; }

        /// <summary>
        /// Gets or sets the HTTP content type of responses created by the health check runner. Must not
        /// have a null or empty value.
        /// </summary>
        public string ContentType
        {
            get => _contentType;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));
                _contentType = value;
            }
        }
        /// <summary>
        /// Gets or sets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Pass"/>. Must have a value in the 200-399 range.
        /// </summary>
        public int PassStatusCode
        {
            get => _passStatusCode;
            set
            {
                if (value < 200 || value > 399)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be in the range of 200-399.");
                _passStatusCode = value;
            }
        }
        /// <summary>
        /// Gets or sets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Warn"/>. Must have a value in the 200-399 range.
        /// </summary>
        public int WarnStatusCode
        {
            get => _warnStatusCode;
            set
            {
                if (value < 200 || value > 399)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be in the range of 200-399.");
                _warnStatusCode = value;
            }
        }
        /// <summary>
        /// Gets or sets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Fail"/>. Must have a value in the 400-599 range.
        /// </summary>
        public int FailStatusCode
        {
            get => _failStatusCode;
            set
            {
                if (value < 400 || value > 599)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be in the range of 400-599.");
                _failStatusCode = value;
            }
        }
    }
}
#endif
