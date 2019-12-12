using Microsoft.Extensions.DependencyInjection;
using System;

namespace RockLib.HealthChecks.DependencyInjection
{
    /// <summary>
    /// Extension methods for dependency injection and health checks.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds an unnamed <see cref="HealthCheckRunner"/> to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IHealthCheckRunnerBuilder AddHealthCheckRunner(this IServiceCollection services,
            Action<IHealthCheckRunnerOptions> configureOptions = null) =>
            services.AddHealthCheckRunner("", configureOptions);

        /// <summary>
        /// Adds a <see cref="HealthCheckRunner"/> with the specified name to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The name of the health check runner to add.</param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="IHealthCheckRunnerOptions"/> object that is used to create the
        /// <see cref="HealthCheckRunner"/>.
        /// </param>
        /// <returns></returns>
        public static IHealthCheckRunnerBuilder AddHealthCheckRunner(this IServiceCollection services,
            string name, Action<IHealthCheckRunnerOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var builder = new HealthCheckRunnerBuilder(name, services, configureOptions);
            
            services.AddSingleton<IHealthCheckRunner>(builder.Build);

            return builder;
        }

        /// <summary>
        /// Adds the specified health check to the builder registrations.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthCheckRunnerBuilder"/>.</param>
        /// <param name="healthCheck">An <see cref="IHealthCheck"/> instance.</param>
        /// <returns>The <see cref="IHealthCheckRunnerBuilder"/>.</returns>
        public static IHealthCheckRunnerBuilder AddHealthCheck(this IHealthCheckRunnerBuilder builder, IHealthCheck healthCheck)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (healthCheck == null)
                throw new ArgumentNullException(nameof(healthCheck));

            return builder.AddHealthCheck(_ => healthCheck);
        }

        /// <summary>
        /// Adds the specified health check to the builder registrations.
        /// </summary>
        /// <typeparam name="THealthCheck">The health check implementation type.</typeparam>
        /// <param name="builder">The <see cref="IHealthCheckRunnerBuilder"/>.</param>
        /// <param name="parameters">Constructor arguments not provided by the <see cref="IServiceProvider"/>.</param>
        /// <returns>The <see cref="IHealthCheckRunnerBuilder"/>.</returns>
        public static IHealthCheckRunnerBuilder AddHealthCheck<THealthCheck>(this IHealthCheckRunnerBuilder builder, params object[] parameters)
            where THealthCheck : class, IHealthCheck
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.AddHealthCheck(serviceProvider => ActivatorUtilities.CreateInstance<THealthCheck>(serviceProvider, parameters));
        }
    }
}
