# :warning: Deprecation Warning :warning:

This library has been deprecated and will no longer receive updates.

---

RockLib has been a cornerstone of our open source efforts here at Rocket Mortgage, and it's played a significant role in our journey to drive innovation and collaboration within our organization and the open source community. It's been amazing to witness the collective creativity and hard work that you all have poured into this project.

However, as technology relentlessly evolves, so must we. The decision to deprecate this library is rooted in our commitment to staying at the cutting edge of technological advancements. While this chapter is ending, it opens the door to exciting new opportunities on the horizon.

We want to express our heartfelt thanks to all the contributors and users who have been a part of this incredible journey. Your contributions, feedback, and enthusiasm have been invaluable, and we are genuinely grateful for your dedication. 🚀

---

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
  - Downstream Services
    - [HttpStatsHealthCheck](docs/HttpStatsHealthCheck.md)
      - A health check that monitors HTTP calls to downstream systems and reports on outcomes.
