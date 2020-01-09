#if NET462 || NETSTANDARD2_0
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RockLib.HealthChecks.DependencyInjection
{
    /// <summary>
    /// Extension methods for dependency injection and health checks.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        private const ServiceLifetime _defaultLifetime = ServiceLifetime.Singleton;

        /// <summary>
        /// Adds an unnamed <see cref="HealthCheckRunner"/> to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">A callback for configuring the <see cref="IHealthCheckRunnerOptions"/>.</param>
        /// <returns>A new <see cref="IHealthCheckRunnerBuilder"/> for registering health checks.</returns>
        public static IHealthCheckRunnerBuilder AddHealthCheckRunner(this IServiceCollection services,
            Action<IHealthCheckRunnerOptions> configureOptions) =>
            services.AddHealthCheckRunner(configureOptions, _defaultLifetime);


        /// <summary>
        /// Adds an unnamed <see cref="HealthCheckRunner"/> to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">A callback for configuring the <see cref="IHealthCheckRunnerOptions"/>.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the health check runner.</param>
        /// <returns>A new <see cref="IHealthCheckRunnerBuilder"/> for registering health checks.</returns>
        public static IHealthCheckRunnerBuilder AddHealthCheckRunner(this IServiceCollection services,
            Action<IHealthCheckRunnerOptions> configureOptions = null,
            ServiceLifetime lifetime = _defaultLifetime) =>
            services.AddHealthCheckRunner("", configureOptions, lifetime);

        /// <summary>
        /// Adds a <see cref="HealthCheckRunner"/> with the specified name to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The name of the health check runner to add.</param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="IHealthCheckRunnerOptions"/> object that is used to create the
        /// <see cref="HealthCheckRunner"/>.
        /// </param>
        /// <returns>A new <see cref="IHealthCheckRunnerBuilder"/> for registering health checks.</returns>
        public static IHealthCheckRunnerBuilder AddHealthCheckRunner(this IServiceCollection services,
            string name, Action<IHealthCheckRunnerOptions> configureOptions) =>
            services.AddHealthCheckRunner(name, configureOptions, _defaultLifetime);

        /// <summary>
        /// Adds a <see cref="HealthCheckRunner"/> with the specified name to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The name of the health check runner to add.</param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="IHealthCheckRunnerOptions"/> object that is used to create the
        /// <see cref="HealthCheckRunner"/>.
        /// </param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the health check runner.</param>
        /// <returns>A new <see cref="IHealthCheckRunnerBuilder"/> for registering health checks.</returns>
        public static IHealthCheckRunnerBuilder AddHealthCheckRunner(this IServiceCollection services,
            string name, Action<IHealthCheckRunnerOptions> configureOptions = null,
            ServiceLifetime lifetime = _defaultLifetime)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var builder = new HealthCheckRunnerBuilder(name, services, configureOptions);
            
            services.Add(new ServiceDescriptor(typeof(IHealthCheckRunner), builder.Build, lifetime));

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
#endif
