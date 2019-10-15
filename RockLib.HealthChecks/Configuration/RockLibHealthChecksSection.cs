using System.Configuration;
using System.Linq;

namespace RockLib.HealthChecks.Configuration
{
    /// <summary>
    /// Defines a configuration section that can create an instance of <see cref="HealthCheckRunner"/>.
    /// </summary>
    public sealed class RockLibHealthChecksSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the configuration section that defines the <see cref="HealthCheckRunner"/>.
        /// </summary>
        [ConfigurationProperty("settings", IsRequired = true)]
        public RockLibSettingsSection Settings => (RockLibSettingsSection)this["settings"];

        /// <summary>
        /// Create an instance of <see cref="HealthCheck"/> based on the values of the <see cref=
        /// "Settings"/> property.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="HealthCheckRunner"/> based on the values of the <see cref=
        /// "Settings"/> property.
        /// </returns>
        public HealthCheckRunner CreateRunner()
        {
            var healthChecks = Settings.HealthChecks.Cast<LateBoundConfigurationElement<IHealthCheck>>()
                .Select(section => section.CreateInstance());

            var responseCustomizer =
                Settings.ResponseCustomizer.ElementInformation.IsPresent
                    ? Settings.ResponseCustomizer.CreateInstance()
                    : null;

            var contentType = Settings.ContentType ?? HealthCheckRunner.DefaultContentType;
            var passStatusCode = Settings.PassStatusCode != -1 ? Settings.PassStatusCode : HealthCheckRunner.DefaultPassStatusCode;
            var warnStatusCode = Settings.WarnStatusCode != -1 ? Settings.WarnStatusCode : HealthCheckRunner.DefaultWarnStatusCode;
            var failStatusCode = Settings.FailStatusCode != -1 ? Settings.FailStatusCode : HealthCheckRunner.DefaultFailStatusCode;

            return new HealthCheckRunner(healthChecks, Settings.Description, Settings.ServiceId,
                Settings.Version, Settings.ReleaseId, responseCustomizer,
                contentType, passStatusCode, warnStatusCode, failStatusCode);
        }
    }
}
