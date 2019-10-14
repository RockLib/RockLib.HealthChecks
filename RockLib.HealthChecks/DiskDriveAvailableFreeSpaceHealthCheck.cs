using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if !(NET35 || NET40)
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RockLib.HealthChecks
{
    /// <summary>
    /// A health check that checks the amount of available free disk storage.
    /// </summary>
    public class DiskDriveAvailableFreeSpaceHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="DiskDriveAvailableFreeSpaceHealthCheck"/> class.
        /// </summary>
        /// <param name="componentName">
        /// The name of the logical downstream dependency or sub-component of a service. Defaults to 'disk'. Must not contain a
        /// colon.
        /// </param>
        /// <param name="measurementName">
        /// The name of the measurement that the status is reported for. Defaults to 'storage'. Must not
        /// contain a colon.
        /// </param>
        /// <param name="componentType">The type of the component.</param>
        /// <param name="componentId">
        /// A unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </param>
        /// <param name="driveName">
        /// The name of the drive on which to check the free disk storage.
        /// The expected format for the C drive would be 'C:\' (at least on Windows).
        /// The wildcard '*' can be used to return results from all drives.
        /// </param>
        /// <param name="warnMinimumFreeGigabytes">
        /// If the amount of free disk storage, in gigabytes, is below this level, set the status to <see cref="HealthStatus.Warn"/>.
        /// </param>
        /// <param name="failMinimumFreeGigabytes">
        /// If the amount of free disk storage, in gigabytes, is below this level, set the status to <see cref="HealthStatus.Fail"/>.
        /// </param>
        public DiskDriveAvailableFreeSpaceHealthCheck(string componentName = "diskDrive", string measurementName = "availableFreeSpace",
            string componentType = "system", string componentId = null,
            string driveName = "*", double warnMinimumFreeGigabytes = 5, double failMinimumFreeGigabytes = .5)
        {
            ComponentName = componentName;
            MeasurementName = measurementName;
            ComponentType = componentType;
            ComponentId = componentId;
            DriveName = driveName;
            WarnMinimumFreeGigabytes = warnMinimumFreeGigabytes;
            FailMinimumFreeGigabytes = failMinimumFreeGigabytes;
        }

        /// <summary>
        /// Gets the name of the logical downstream dependency or sub-component of a service. Must not
        /// contain a colon.
        /// </summary>
        public string ComponentName { get; }

        /// <summary>
        /// Gets the name of the measurement that the status is reported for. Must not contain a colon.
        /// </summary>
        public string MeasurementName { get; }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        public string ComponentType { get; }

        /// <summary>
        /// Gets a unique identifier of an instance of a specific sub-component/dependency of a service.
        /// </summary>
        public string ComponentId { get; }

        /// <summary>
        /// The name of the drive on which to check the free disk storage.
        /// </summary>
        public string DriveName { get; }

        /// <summary>
        /// If the amount of free disk storage, in gigabytes, is below this level, set the status to <see cref="HealthStatus.Warn"/>.
        /// </summary>
        public double WarnMinimumFreeGigabytes { get; }

        /// <summary>
        /// If the amount of free disk storage, in gigabytes, is below this level, set the status to <see cref="HealthStatus.Fail"/>.
        /// </summary>
        public double FailMinimumFreeGigabytes { get; }


#if NET35 || NET40
        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <returns>A list of health check results.</returns>
        public IList<HealthCheckResult> Check() => GetResults();
#else
        /// <summary>
        /// Check the health of the sub-component/dependency.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// A task list of health check results representing the asynchronous operation.
        /// </returns>
        public Task<IReadOnlyList<HealthCheckResult>> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult((IReadOnlyList<HealthCheckResult>)GetResults());
#endif

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
                    .FirstOrDefault(d => string.Equals(d.Name, DriveName, StringComparison.InvariantCultureIgnoreCase));
                return new List<HealthCheckResult>() { GetResult(drive) };

            }           
        }

        private HealthCheckResult GetResult(DriveInfo drive)
        {
            var result = this.CreateHealthCheckResult();

            if (drive == null)
            {
                result.Status = HealthStatus.Fail;
                result.Output = $"Configured drive {DriveName} is not present on system";
                return result;
            }

            var availableFreeSpace = ((double)drive.AvailableFreeSpace) / 1024 / 1024 / 1024;

            if (availableFreeSpace < FailMinimumFreeGigabytes)
            {
                result.Status = HealthStatus.Fail;
                result.Output = $"Minimum configured 'Fail' gigabytes for disk {drive.Name} is {FailMinimumFreeGigabytes} but the actual free space is {availableFreeSpace} gigabytes";
            }
            else if (availableFreeSpace < WarnMinimumFreeGigabytes)
            {
                result.Status = HealthStatus.Warn;
                result.Output = $"Minimum configured 'Warn' gigabytes for disk {drive.Name} is {WarnMinimumFreeGigabytes} but the actual free space is {availableFreeSpace} gigabytes";
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
