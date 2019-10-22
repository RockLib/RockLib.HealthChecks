# How to instantiate a health check runner

A `HealthCheckRunner` object can be instantiated directly with its constructor.

Parameter          | Default Value               | Description
------------------ | --------------------------- | -----------
healthChecks       | `null`                      | The collection of `IHealthCheck` objects to be checked by this runner.
name               | `null`                      | The name of the runner.
description        | `null`                      | The human-friendly description of the service.
serviceId          | `null`                      | The unique identifier of the service, in the application scope.
version            | `null`                      | The public version of the service.
releaseId          | `null`                      | The "release version" or "release ID" of the service.
responseCustomizer | `null`                      | The `IHealthResponseCustomizer` that customizes each `HealthResponse` object returned by this runner.
contentType        | `"application/health+json"` | The HTTP content type of responses created by this health check runner. Must not have a null or empty value.
passStatusCode     | `200`                       | The HTTP status code of responses created by this health check runner that have a status of `Pass`. Must have a value in the 200-399 range.
warnStatusCode     | `200`                       | The HTTP status code of responses created by this health check runner that have a status of `Warn`. Must have a value in the 200-399 range.
failStatusCode     | `503`                       | The HTTP status code of responses created by this health check runner that have a status of `Fail`. Must have a value in the 400-599 range.

---

This example creates an empty runner - one that always passes.

```c#
IHealthCheckRunner runner = new HealthCheckRunner();
```

---

This example creates a runner with multiple health checks:

```c#
IHealthCheckRunner runner = new HealthCheckRunner(
    healthChecks: new IHealthCheck[]
    {
        new DiskDriveHealthCheck(warnGigabytes: 30, failGigabytes: 5),
        new SystemUptimeHealthCheck()
    },
    description: "Example");
```
