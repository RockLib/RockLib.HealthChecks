#if NET462 || NETSTANDARD2_0
using RockLib.HealthChecks.DependencyInjection;
using System;

namespace RockLib.HealthChecks.System
{
    /// <summary>
    /// Defines extension methods for registering system health checks with a
    /// <see cref="IHealthCheckRunnerBuilder"/>.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds a <see cref="DiskDriveHealthCheck"/> to the builder registrations.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthCheckRunnerBuilder"/>.</param>
        /// <param name="warnGigabytes">
        /// The the lowest allowable level of available free space in gigabytes, below which results in a <see cref="HealthStatus.Warn"/> status.
        /// </param>
        /// <param name="failGigabytes">
        /// The the lowest allowable level of available free space in gigabytes, below which results in a <see cref="HealthStatus.Fail"/> status.
        /// </param>
        /// <param name="driveName">
        /// The name of the drive on which to check the available free space.
        /// The expected format for the C drive would be 'C:\' (at least on Windows).
        /// The wildcard '*' can be used to return results from all drives.
        /// </param>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Defaults to
        /// 'diskDrive'. Must not contain a colon.
        /// </param>
        /// <param name="measurementName">
        /// The name of the measurement that the status is reported for. Defaults to 'availableFreeSpace'.
        /// Must not contain a colon.
        /// </param>
        /// <param name="componentType">The type of the component. Defaults to 'system'.</param>
        /// <param name="componentId">
        /// A unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </param>
        /// <returns>The <see cref="IHealthCheckRunnerBuilder"/>.</returns>
        public static IHealthCheckRunnerBuilder AddDiskDriveHealthCheck(this IHealthCheckRunnerBuilder builder,
            double warnGigabytes = 5, double failGigabytes = .5, string driveName = "*",
            string componentName = "diskDrive", string measurementName = "availableFreeSpace",
            string componentType = "system", string componentId = null)
        {
            if (string.IsNullOrEmpty(driveName)) throw new ArgumentNullException(nameof(driveName));
            if (warnGigabytes < 0) throw new ArgumentOutOfRangeException(nameof(warnGigabytes), "Must not be less than zero.");
            if (failGigabytes < 0) throw new ArgumentOutOfRangeException(nameof(failGigabytes), "Must not be less than zero.");

            return builder.AddHealthCheck(serviceProvider =>
                new DiskDriveHealthCheck(warnGigabytes, failGigabytes, driveName, componentName, measurementName, componentType, componentId));
        }

        /// <summary>
        /// Adds a <see cref="ProcessUptimeHealthCheck"/> to the builder registrations.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthCheckRunnerBuilder"/>.</param>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Defaults to 'process'.
        /// Must not contain a colon.
        /// </param>
        /// <param name="measurementName">
        /// The name of the measurement that the status is reported for. Defaults to 'uptime'. Must not
        /// contain a colon.
        /// </param>
        /// <param name="componentType">The type of the component. Defaults to 'system'.</param>
        /// <param name="componentId">
        /// A unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </param>
        /// <returns>The <see cref="IHealthCheckRunnerBuilder"/>.</returns>
        public static IHealthCheckRunnerBuilder AddProcessUptimeHealthCheck(this IHealthCheckRunnerBuilder builder,
            string componentName = "process", string measurementName = "uptime",
            string componentType = "system", string componentId = null)
        {
            return builder.AddHealthCheck(serviceProvider =>
                new ProcessUptimeHealthCheck(componentName, measurementName, componentType, componentId));
        }

        /// <summary>
        /// Adds a <see cref="SystemUptimeHealthCheck"/> to the builder registrations.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthCheckRunnerBuilder"/>.</param>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Defaults to 'system'.
        /// Must not contain a colon.
        /// </param>
        /// <param name="measurementName">
        /// The name of the measurement that the status is reported for. Defaults to 'uptime'. Must not
        /// contain a colon.
        /// </param>
        /// <param name="componentType">The type of the component. Defaults to 'system'.</param>
        /// <param name="componentId">
        /// A unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </param>
        /// <returns>The <see cref="IHealthCheckRunnerBuilder"/>.</returns>
        public static IHealthCheckRunnerBuilder AddSystemUptimeHealthCheck(this IHealthCheckRunnerBuilder builder,
            string componentName = "system", string measurementName = "uptime",
            string componentType = "system", string componentId = null)
        {
            return builder.AddHealthCheck(serviceProvider =>
                new SystemUptimeHealthCheck(componentName, measurementName, componentType, componentId));
        }
    }
}
#endif
