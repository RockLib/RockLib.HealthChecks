using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace RockLib.HealthChecks.DependencyInjection
{
    /// <summary>
    /// The default implementation of the <see cref="IHealthCheckRunnerBuilder"/> interface.
    /// </summary>
    public class HealthCheckRunnerBuilder : IHealthCheckRunnerBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckRunnerBuilder"/> class.
        /// </summary>
        /// <param name="name">The name of <see cref="HealthCheckRunner"/> to be created by this builder.</param>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> that is configured when the <see cref="AddHealthCheck"/> method is called.
        /// </param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="IHealthCheckRunnerOptions"/> object that is used to create the
        /// <see cref="HealthCheckRunner"/>.
        /// </param>
        public HealthCheckRunnerBuilder(string name, IServiceCollection services, Action<IHealthCheckRunnerOptions> configureOptions = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            ConfigureOptions = configureOptions;

            Services.AddOptions<HealthCheckRunnerOptions>(Name);
        }

        /// <summary>
        /// Gets the name of the <see cref="HealthCheckRunner"/> to be created by this builder.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> that is configured when the <see cref="AddHealthCheck"/> method
        /// is called.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Gets the delegate to configure the <see cref="IHealthCheckRunnerOptions"/> object that is used to create the
        /// <see cref="HealthCheckRunner"/>.
        /// </summary>
        public Action<IHealthCheckRunnerOptions> ConfigureOptions { get; }

        /// <summary>
        /// Adds a registration delegate for creating a health check.
        /// </summary>
        /// <param name="registration">The registration delegate.</param>
        public IHealthCheckRunnerBuilder AddHealthCheck(Func<IServiceProvider, IHealthCheck> registration)
        {
            Services.Configure<HealthCheckRunnerOptions>(Name, options => options.Registrations.Add(registration));
            return this;
        }

        /// <summary>
        /// Creates an instance of <see cref="HealthCheckRunner"/> with 
        /// </summary>
        /// <param name="serviceProvider">
        /// The <see cref="IServiceProvider"/> that retrieves the services required to create the <see cref="HealthCheckRunner"/>.
        /// </param>
        /// <returns></returns>
        public HealthCheckRunner Build(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<HealthCheckRunnerOptions>>();

            var options = optionsMonitor.Get(Name); // TODO: What happens if the named options don't exist?
            ConfigureOptions?.Invoke(options);

            var healthChecks = options.Registrations.Select(registration => registration.Invoke(serviceProvider));

            return new HealthCheckRunner(healthChecks, Name,
                options.Description, options.ServiceId, options.Version, options.ReleaseId,
                options.ResponseCustomizer, options.ContentType, options.PassStatusCode,
                options.WarnStatusCode, options.FailStatusCode);
        }
    }
}
