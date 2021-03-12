﻿#if NET462 || NETSTANDARD2_0 || NET5_0
namespace RockLib.HealthChecks.DependencyInjection
{
    /// <summary>
    /// A builder used to register health checks.
    /// </summary>
    public interface IHealthCheckRunnerBuilder
    {
        /// <summary>
        /// Adds a health check registration delegate to the builder.
        /// </summary>
        /// <param name="registration">The health check registration delegate.</param>
        IHealthCheckRunnerBuilder AddHealthCheck(HealthCheckRegistration registration);
    }
}
#endif
