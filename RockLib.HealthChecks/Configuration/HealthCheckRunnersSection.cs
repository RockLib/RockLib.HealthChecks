#if NET35 || NET40 || NET45
using System.Configuration;

namespace RockLib.HealthChecks.Configuration
{
    /// <summary>
    /// Defines a configuration element collection with elements of type <see cref="HealthCheckRunnerSection"/>.
    /// </summary>
    [ConfigurationCollection(typeof(HealthCheckRunnerSection), AddItemName = "runner")]
    public sealed class HealthCheckRunnersSection : ConfigurationElementCollection
    {
        /// <inheritdoc/>
        protected override ConfigurationElement CreateNewElement()
        {
            return new HealthCheckRunnerSection();
        }

        /// <inheritdoc/>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HealthCheckRunnerSection)element).Name ?? "default";
        }
    }
}
#endif
