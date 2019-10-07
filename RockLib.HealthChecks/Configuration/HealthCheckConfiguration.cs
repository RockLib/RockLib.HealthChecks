using System;
using System.Configuration;

namespace RockLib.HealthChecks.Configuration
{
    /// <summary>
    /// Defines a configuration section that defines a collection of <see cref="IHealthCheck"/> objects.
    /// </summary>
    [ConfigurationCollection(typeof(LateBoundConfigurationElement<IHealthCheck>), AddItemName = "healthCheck")]
    public sealed class HealthChecksSection : ConfigurationElementCollection
    {
        /// <inheritdoc/>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LateBoundConfigurationElement<IHealthCheck>();
        }

        /// <inheritdoc/>
        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element is LateBoundConfigurationElement<IHealthCheck> lateBoundElement)
                return lateBoundElement.SerializeToXml();
            throw new InvalidOperationException($"Unknown element type: {element.GetType()}");
        }
    }
}
