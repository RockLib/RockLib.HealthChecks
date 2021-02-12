#if NET35 || NET40 || NET45
using System.Configuration;

namespace RockLib.HealthChecks.Configuration
{
    /// <summary>
    /// Defines a configuration section that contains the information necessary to create an
    /// instance of <see cref="HealthCheckRunner"/>.
    /// </summary>
    public sealed class HealthCheckRunnerSection : ConfigurationElement
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name => NoEmptyString((string)this["name"]);

        /// <summary>
        /// Gets the public version of the service.
        /// </summary>
        [ConfigurationProperty("version", IsRequired = false)]
        public string Version => NoEmptyString((string)this["version"]);

        /// <summary>
        /// Gets the "release version" or "release ID" of the service.
        /// </summary>
        [ConfigurationProperty("releaseId", IsRequired = false)]
        public string ReleaseId => NoEmptyString((string)this["releaseId"]);

        /// <summary>
        /// Gets the unique identifier of the service, in the application scope.
        /// </summary>
        [ConfigurationProperty("serviceId", IsRequired = false)]
        public string ServiceId => NoEmptyString((string)this["serviceId"]);

        /// <summary>
        /// Gets the human-friendly description of the service.
        /// </summary>
        [ConfigurationProperty("description", IsRequired = false)]
        public string Description => NoEmptyString((string)this["description"]);

        /// <summary>
        /// Gets the HTTP content type of responses created by the health check runner.
        /// </summary>
        [ConfigurationProperty("contentType", IsRequired = false)]
        public string ContentType => NoEmptyString((string)this["contentType"]);

        /// <summary>
        /// Gets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Pass"/>.
        /// </summary>
        [ConfigurationProperty("passStatusCode", IsRequired = false, DefaultValue = -1)]
        public int PassStatusCode => (int)this["passStatusCode"];

        /// <summary>
        /// Gets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Warn"/>.
        /// </summary>
        [ConfigurationProperty("warnStatusCode", IsRequired = false, DefaultValue = -1)]
        public int WarnStatusCode => (int)this["warnStatusCode"];

        /// <summary>
        /// Gets the HTTP status code of responses created by the health check runner that have a
        /// status of <see cref="HealthStatus.Fail"/>.
        /// </summary>
        [ConfigurationProperty("failStatusCode", IsRequired = false, DefaultValue = -1)]
        public int FailStatusCode => (int)this["failStatusCode"];

        /// <summary>
        /// Gets the <see cref="HealthStatus"/> for the <see cref="HealthCheckResult"/> that is
        /// returned because an <see cref="IHealthCheck"/> has thrown an exception. An undefined
        /// value is treated as <see langword="null"/>.
        /// </summary>
        [ConfigurationProperty("uncaughtExceptionStatus", IsRequired = false, DefaultValue = HealthCheckRunner.DefaultUncaughtExceptionStatus)]
        public HealthStatus UncaughtExceptionStatus => (HealthStatus)this["uncaughtExceptionStatus"];

        /// <summary>
        /// Gets the section that defines the list of <see cref="IHealthCheck"/> objects that are
        /// checked by the runner.
        /// </summary>
        [ConfigurationProperty("healthChecks", IsRequired = false)]
        public HealthChecksSection HealthChecks => (HealthChecksSection)this["healthChecks"];

        /// <summary>
        /// Gets the section that defines the <see cref="IHealthResponseCustomizer"/> that customizes
        /// each <see cref="HealthResponse"/> object returned by the runner.
        /// </summary>
        [ConfigurationProperty("responseCustomizer", IsRequired = false)]
        public LateBoundConfigurationElement<IHealthResponseCustomizer> ResponseCustomizer => (LateBoundConfigurationElement<IHealthResponseCustomizer>)this["responseCustomizer"];

        private static string NoEmptyString(string value)
        {
            if (value == "") return null;
            return value;
        }
    }
}
#endif
