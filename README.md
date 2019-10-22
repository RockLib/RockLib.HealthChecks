# RockLib.HealthChecks

*An implementation of the draft health check RFC located at https://inadarei.github.io/rfc-healthcheck/.*

---

- [Getting started](docs/GettingStarted.md)
- How to:
  - [Manually perform health checks](docs/ManualHealthChecks.md)
  - [Add RockLib.HealthChecks to an ASP.NET Core application](docs/AspNetCore.md)
  - [Add RockLib.HealthChecks to an ASP.NET WebApi application](docs/WebApi.md)
  - [Add RockLib.HealthChecks to an ASP.NET application via HttpModule](docs/HttpModule.md)
  - [Format the output of Microsoft.Extensions.Diagnostics.HealthChecks in the draft RFC format](docs/ResponseWriter.md)
  - [Instantiate health check runners](docs/InstantiateRunner.md)
  - [Configure an application with appsettings.json](docs/ConfigureAppSettings.md)
  - [Configure a .NET Framework application (4.5 or below) with app.config/web.config](docs/ConfigureNet45AndBelow.md)
  - [Configure a .NET Framework application (4.5.1 or above) with app.config/web.config](docs/ConfigureNet451AndAbove.md)
  - [Create custom health checks](docs/CustomHealthCheck.md)
- Existing Health Checks:
  - System
    - [DiskDriveHealthCheck](docs/DiskDriveHealthCheck.md)
      - A health check that monitors the amount of available free space on disk.
    - [ProcessUptimeHealthCheck](docs/ProcessUptimeHealthCheck.md)
      - A health check that records the uptime of the current process. Always passes.
    - [SystemUptimeHealthCheck](docs/SystemUptimeHealthCheck.md)
      - A health check that records the uptime of the system. Always passes.
