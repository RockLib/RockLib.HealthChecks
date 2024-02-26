using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.System
{
    /// <summary>
    /// A health check that monitors the amount of available free space on disk.
    /// </summary>
    public class DiskDriveHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskDriveHealthCheck"/> class.
        /// </summary>
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
        public DiskDriveHealthCheck(double warnGigabytes = 5, double failGigabytes = .5, string driveName = "*",
            string componentName = "diskDrive", string measurementName = "availableFreeSpace",
            string componentType = "system", string? componentId = null)
        {
            if (warnGigabytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(warnGigabytes), "Must not be less than zero.");
            }
            if (failGigabytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(failGigabytes), "Must not be less than zero.");
            }
            DriveName = !string.IsNullOrEmpty(driveName) ? driveName : throw new ArgumentNullException(nameof(driveName));
            WarnGigabytes = warnGigabytes;
            FailGigabytes = failGigabytes;
            ComponentName = componentName;
            MeasurementName = measurementName;
            ComponentType = componentType;
            ComponentId = componentId;
        }

        /// <summary>
        /// The name of the drive on which to check the free disk storage.
        /// </summary>
        public string DriveName { get; }

        /// <summary>
        /// Gets the the lowest allowable level of available free space in gigabytes, below which results
        /// in a <see cref="HealthStatus.Warn"/> status.
        /// </summary>
        public double WarnGigabytes { get; }

        /// <summary>
        /// Gets the the lowest allowable level of available free space in gigabytes, below which results
        /// in a <see cref="HealthStatus.Fail"/> status.
        /// </summary>
        public double FailGigabytes { get; }

        /// <summary>
        /// Gets the name of the logical downstream dependency or sub-component of a service.
        /// </summary>
        public string ComponentName { get; }

        /// <summary>
        /// Gets the name of the measurement that the status is reported for.
        /// </summary>
        public string MeasurementName { get; }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        public string ComponentType { get; }

        /// <summary>
        /// Gets a unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </summary>
        public string? ComponentId { get; }

        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// A task list of health check results representing the asynchronous operation.
        /// </returns>
        public Task<IReadOnlyList<HealthCheckResult>> CheckAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((IReadOnlyList<HealthCheckResult>)GetResults());

        private List<HealthCheckResult> GetResults()
        {
            var results = new List<HealthCheckResult>();

            if (DriveName == "*")
            {
                var drives = DriveInfo.GetDrives();
                return drives.Select(d => GetResult(d)).ToList();
            }
            else
            {
                var drive = DriveInfo.GetDrives()
                    .FirstOrDefault(d => string.Equals(d.Name, DriveName, StringComparison.OrdinalIgnoreCase));
                return [GetResult(drive)];
            }
        }

        private HealthCheckResult GetResult(DriveInfo? drive)
        {
            var result = this.CreateHealthCheckResult();

            if (drive is null)
            {
                result.Status = HealthStatus.Warn;
                result.Output = $"Configured drive {DriveName} is not present on system.";
                return result;
            }

            if (!drive.IsReady)
            {
                result.Status = HealthStatus.Warn;
                result.Output = $"Configured drive {DriveName} is not ready.";
                return result;
            }

            double availableFreeSpace;
            try
            {
                const double gigabytes = 1024 * 1024 * 1024;
                availableFreeSpace = drive.AvailableFreeSpace / gigabytes;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                result.Status = HealthStatus.Warn;
                result.Output = $"Error reading available free space on drive {DriveName}. {ex.GetType().Name}: {ex.Message}";
                return result;
            }

            if (availableFreeSpace < FailGigabytes)
            {
                result.Status = HealthStatus.Fail;
                result.Output = $"Configured FailGigabytes for disk {drive.Name} is {FailGigabytes:#.###} but the actual free space is {availableFreeSpace:#.###} gigabytes.";
            }
            else if (availableFreeSpace < WarnGigabytes)
            {
                result.Status = HealthStatus.Warn;
                result.Output = $"Configured WarnGigabytes for disk {drive.Name} is {WarnGigabytes:#.###} but the actual free space is {availableFreeSpace:#.###} gigabytes.";
            }
            else
            {
                result.Status = HealthStatus.Pass;
            }

            result.ObservedValue = availableFreeSpace;
            result.ObservedUnit = "GB";
            result["driveName"] = drive.Name;
            return result;
        }
    }
}
