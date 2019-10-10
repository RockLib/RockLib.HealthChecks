using System;
using System.Diagnostics;
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
    public class DiskStorageHealthCheck : SingleResultHealthCheck
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="DiskStorageHealthCheck"/> class.
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
        /// <param name="driveName">The name of the drive on which to check the free disk storage.</param>
        /// <param name="warnMinimumFreeGigabytes">
        /// If the amount of free disk storage, in gigabytes, is below this level, set the status to <see cref="HealthStatus.Warn"/>.
        /// </param>
        /// <param name="failMinimumFreeGigabytes">
        /// If the amount of free disk storage, in gigabytes, is below this level, set the status to <see cref="HealthStatus.Fail"/>.
        /// </param>
        public DiskStorageHealthCheck(string componentName = "disk", string measurementName = "storage",
            string componentType = null, string componentId = null, 
            string driveName = "C:\\", double warnMinimumFreeGigabytes = 1, double failMinimumFreeGigabytes = .25)
            : base(componentName, measurementName, componentType, componentId)
        {
            DriveName = driveName;
            WarnMinimumFreeGigabytes = warnMinimumFreeGigabytes;
            FailMinimumFreeGigabytes = failMinimumFreeGigabytes;
        }

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
        /// <inheritdoc/>
        protected override void Check(HealthCheckResult result)
        {
            SetResult(result);
        }
#else
        /// <inheritdoc/>
        protected override Task CheckAsync(HealthCheckResult result, CancellationToken cancellationToken)
        {
            SetResult(result);
            return Task.FromResult(0);
        }
#endif

        private void SetResult(HealthCheckResult result)
        {
            var drive = DriveInfo.GetDrives()
                .FirstOrDefault(d => string.Equals(d.Name, DriveName, StringComparison.InvariantCultureIgnoreCase));

            if (drive == null)
            {
                result.Status = HealthStatus.Fail;
                result.Output = $"Configured drive {DriveName} is not present on system";
                return;
            }

            var availableFreeSpace = ((double)drive.AvailableFreeSpace) / 1024 / 1024 / 1024;

            if (availableFreeSpace < FailMinimumFreeGigabytes)
            {
                result.Status = HealthStatus.Fail;
                result.Output = $"Minimum configured 'Fail' gigabytes for disk {DriveName} is {FailMinimumFreeGigabytes} but the actual free space is {availableFreeSpace} gigabytes";
            }
            else if (availableFreeSpace  < WarnMinimumFreeGigabytes)
            {
                result.Status = HealthStatus.Warn;
                result.Output = $"Minimum configured 'Warn' gigabytes for disk {DriveName} is {WarnMinimumFreeGigabytes} but the actual free space is {availableFreeSpace} gigabytes";
            }
            else
            {
                result.Status = HealthStatus.Pass;
            }

            result.ObservedValue = availableFreeSpace;
            result.ObservedUnit = "GB";
        }
    }
}
