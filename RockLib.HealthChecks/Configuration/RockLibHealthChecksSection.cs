using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RockLib.HealthChecks.Configuration
{
    /// <summary>
    /// Defines a configuration section that can create a collection of <see cref="IHealthCheckRunner"/>
    /// objects.
    /// </summary>
    public sealed class RockLibHealthChecksSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the <see cref="HealthCheckRunnersSection"/> that defines the collection of <see cref=
        /// "IHealthCheckRunner"/> objects returned by the <see cref="CreateRunners"/> method.
        /// </summary>
        [ConfigurationProperty("runners", IsRequired = false)]
        public HealthCheckRunnersSection Runners => (HealthCheckRunnersSection)this["runners"];

        /// <summary>
        /// Creates a collection of <see cref="IHealthCheckRunner"/> objects based on the values of the <see cref=
        /// "Runners"/> property.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="HealthCheckRunner"/> objects based on the values of the <see cref=
        /// "Runners"/> property.
        /// </returns>
        public IList<IHealthCheckRunner> CreateRunners()
        {
            return Runners.Cast<HealthCheckRunnerSection>().Select(runnerSection =>
            {
                var healthChecks = runnerSection.HealthChecks.Cast<LateBoundConfigurationElement<IHealthCheck>>()
                    .Select(section => section.CreateInstance());

                var responseCustomizer =
                    runnerSection.ResponseCustomizer.ElementInformation.IsPresent
                        ? runnerSection.ResponseCustomizer.CreateInstance()
                        : null;

                var contentType = runnerSection.ContentType ?? HealthCheckRunner.DefaultContentType;
                var passStatusCode = runnerSection.PassStatusCode != -1 ? runnerSection.PassStatusCode : HealthCheckRunner.DefaultPassStatusCode;
                var warnStatusCode = runnerSection.WarnStatusCode != -1 ? runnerSection.WarnStatusCode : HealthCheckRunner.DefaultWarnStatusCode;
                var failStatusCode = runnerSection.FailStatusCode != -1 ? runnerSection.FailStatusCode : HealthCheckRunner.DefaultFailStatusCode;

                return (IHealthCheckRunner)new HealthCheckRunner(healthChecks, runnerSection.Name, runnerSection.Description, runnerSection.ServiceId,
                    runnerSection.Version, runnerSection.ReleaseId, responseCustomizer, contentType,
                    passStatusCode, warnStatusCode, failStatusCode);
            }).ToList();
        }
    }
}
