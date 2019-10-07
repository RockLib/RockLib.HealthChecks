using RockLib.Immutable;
using System;
#if NET35 || NET40 || NET45
using RockLib.HealthChecks.Configuration;
using System.Configuration;
#else
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
#endif

namespace RockLib.HealthChecks
{
#if NET35 || NET40 || NET45
    /// <summary>
    /// A static class that contains a <see cref="Runner"/> property of type <see cref="IHealthCheckRunner"/>.
    /// By default, the runner is created from the 'rockLib.healthChecks' configuration section of type
    /// <see cref="RockLibHealthChecksSection"/> from the <see cref="ConfigurationManager"/> static class.
    /// </summary>
#else
    /// <summary>
    /// A static class that contains a <see cref="Runner"/> property of type <see cref="IHealthCheckRunner"/>.
    /// By default, the runner is created from the 'RockLib.HealthChecks' / 'RockLib_HealthChecks' composite
    /// configuration section from the <see cref="Config"/> static class.
    /// </summary>
#endif
    public static class HealthCheck
    {
        private static readonly Semimutable<IHealthCheckRunner> _runner = new Semimutable<IHealthCheckRunner>(GetDefaultRunner);

        /// <summary>
        /// Gets or sets the <see cref="IHealthCheckRunner"/>. Note that this property cannot be set
        /// after it has been accessed.
        /// </summary>
        public static IHealthCheckRunner Runner
        {
            get => _runner.Value;
            set => _runner.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Sets the <see cref="Runner"/> property to the value returned by the function. Note
        /// that this method cannot be called after the <see cref="Runner"/> property has been
        /// accessed.
        /// </summary>
        /// <param name="getRunner">
        /// A function that returns the <see cref="IHealthCheckRunner"/> to be the value of
        /// the <see cref="Runner"/> property.
        /// </param>
        public static void SetRunner(Func<IHealthCheckRunner> getRunner) =>
            _runner.SetValue(getRunner ?? throw new ArgumentNullException(nameof(getRunner)));

        /// <summary>
        /// Resets the runner to its default value, to be loaded from configuration. Note that
        /// this method cannot be called after the <see cref="Runner"/> property has been accessed.
        /// </summary>
        public static void ResetRunner() => _runner.ResetValue();

        private static IHealthCheckRunner GetDefaultRunner()
        {
#if NET35 || NET40 || NET45
            var healtchChecksSection = (RockLibHealthChecksSection)ConfigurationManager.GetSection("rockLib.healthChecks");
            return healtchChecksSection.CreateRunner();
#else
            var section = Config.Root.GetCompositeSection("RockLib.HealthChecks", "RockLib_HealthChecks");
            return section.CreateReloadingProxy<IHealthCheckRunner>(
                    new DefaultTypes { { typeof(IHealthCheckRunner), typeof(HealthCheckRunner) } });
#endif
        }
    }
}
