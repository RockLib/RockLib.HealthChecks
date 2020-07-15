# Getting Started

In this tutorial, we will be building a web application that has a basic health check.

---

Create a ASP.NET Core 2.2 (or above) web application named "HealthCheckTutorial".

---

Add a nuget reference for "RockLib.HealthChecks.AspNetCore" the project.

---

Add a new JSON file to the project named 'appsettings.json'. Set its 'Copy to Output Directory' setting to 'Copy if newer'. Add the following configuration:

```json
{
  "RockLib.HealthChecks": {
    "version": "1",
    "serviceId": "3390b579-d076-4610-a9bd-7d1a5af893f9",
    "description": "my health check",
    "healthChecks": {
      "type": "RockLib.HealthChecks.System.ProcessUptimeHealthCheck, RockLib.HealthChecks"
    }
  }
}
```

---

Edit the `Startup.cs` file as follows:

```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using RockLib.HealthChecks.AspNetCore;

namespace HealthCheckTutorial
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRockLibHealthChecks();
        }
    }
}
```

---

In the project properties 'Debug' section, set the relative url to 'health'.

---

Start the app. It should open a web browser and navigate to the health end point with the following output:

```json
{
  "status": "pass",
  "version": "1",
  "serviceId": "3390b579-d076-4610-a9bd-7d1a5af893f9",
  "description": "my health check",
  "checks": {
    "process:uptime": [
      {
        "observedValue": 5.0565182,
        "observedUnit": "s",
        "status": "pass",
        "componentType": "system",
        "time": "2019-10-18T15:50:20.0340741Z"
      }
    ]
  }
}

```
