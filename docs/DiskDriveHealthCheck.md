# DiskDriveHealthCheck

This health check monitors the amount of available free space on disk. If a disk's available free space is below `WarnGigabytes`, results will have a status of `Warn`. If below `FailGigabytes`, results will have a status of `Fail`.

None of its constructor's parameters are required.

Parameter          | Default Value          | Description
------------------ | ---------------------- | -----------
warnGigabytes      | `5`                    | The the lowest allowable level of available free space in gigabytes, below which results in a `Warn` status.
failGigabytes      | `0.5`                  | The the lowest allowable level of available free space in gigabytes, below which results in a `Fail` status.
driveName          | `"*"`                  | The name of the drive on which to check the available free space. The expected format for the C drive is `"C:\"` (for Windows). The wildcard `"*"` can be used to return results from all drives.
componentName      | `"diskDrive"`          | The name of the logical downstream dependency or sub-component of a service. Must not contain a colon.
measurementName    | `"availableFreeSpace"` | The name of the measurement that the status is reported for. Must not contain a colon.
componentType      | `"system"`             | The type of the component.
componentId        | `null`                 | A unique identifier of an instance of a specific sub-component/dependency of a service.
