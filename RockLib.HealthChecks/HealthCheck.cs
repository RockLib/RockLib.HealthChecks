using System;
using System.Linq;
using IReadOnlyListOfIHealthCheckRunner = System.Collections.Generic.IReadOnlyList<RockLib.HealthChecks.IHealthCheckRunner>;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;

namespace RockLib.HealthChecks
{
    /// <summary>
    /// A static class that defines a collection of <see cref="IHealthCheckRunner"/> objects to be used
    /// by an application. By default, the runners are created from the 'RockLib.HealthChecks' / 'RockLib_HealthChecks'
    /// composite configuration section from the <see cref="Config"/> static class.
    /// </summary>
    public static class HealthCheck
    {
        private static bool _isRunnersLoaded;
        private static IReadOnlyListOfIHealthCheckRunner? _runners;

        /// <summary>
        /// Gets or sets a collection of <see cref="IHealthCheckRunner"/> objects to be used by an
        /// application. Note that this property cannot be set after it has been accessed.
        /// </summary>
        public static IReadOnlyListOfIHealthCheckRunner Runners
        {
            get
            {
                if (!_isRunnersLoaded)
                {
                    _runners = GetDefaultRunners();
                    _isRunnersLoaded = true;
                }

                return _runners!;
            }

            set
            {
                if (_isRunnersLoaded)
                {
                    throw new InvalidOperationException("The Runners property has already been accessed.");
                }

                _isRunnersLoaded = true;
                _runners = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Sets the <see cref="Runners"/> property to the value returned by the function. Note
        /// that this method cannot be called after the <see cref="Runners"/> property has been
        /// accessed.
        /// </summary>
        /// <param name="getRunners">
        /// A function that returns the collection of <see cref="IHealthCheckRunner"/> objects
        /// to be the value of the <see cref="Runners"/> property.
        /// </param>
        public static void SetRunners(Func<IReadOnlyListOfIHealthCheckRunner> getRunners) =>
            Runners = getRunners?.Invoke() ?? throw new ArgumentNullException(nameof(getRunners));

        /// <summary>
        /// Gets the <see cref="IHealthCheckRunner"/> from the <see cref="Runners"/> property by name.
        /// Returns null if a runner cannot be found.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="IHealthCheckRunner"/> to retrieve, or <see langword="null"/> to
        /// retrieve the unnamed or "default" runner.
        /// </param>
        /// <returns></returns>
        public static IHealthCheckRunner? GetRunner(string? name = null) =>
            Runners.FirstOrDefault(r => GetName(r.Name).Equals(GetName(name), StringComparison.OrdinalIgnoreCase));

        private static string GetName(string? name) => string.IsNullOrEmpty(name) ? "default" : name!;

        private static IReadOnlyListOfIHealthCheckRunner GetDefaultRunners()
        {
            var section = Config.Root!.GetCompositeSection("RockLib_HealthChecks", "RockLib.HealthChecks");
            return section.Create<IReadOnlyListOfIHealthCheckRunner>()!;
        }
    }
}