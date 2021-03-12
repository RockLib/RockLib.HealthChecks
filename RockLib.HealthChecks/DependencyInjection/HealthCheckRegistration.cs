#if NET462 || NETSTANDARD2_0 || NET5_0
using System;

namespace RockLib.HealthChecks.DependencyInjection
{
    /// <summary>
    /// Represents a method that creates an <see cref="IHealthCheck"/>.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> that resolves dependencies necessary for the creation of the
    /// <see cref="IHealthCheck"/>.
    /// </param>
    /// <returns>An <see cref="IHealthCheck"/>.</returns>
    public delegate IHealthCheck HealthCheckRegistration(IServiceProvider serviceProvider);

}
#endif
