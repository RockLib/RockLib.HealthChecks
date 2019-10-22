# How to manually perform health checks

The simplest way to manually run a set of health checks is to call `RunAsync` method on a `HealthCheckRunner`.  A runner can be instantiated manually, see [here](InstantiateRunner.md), or it can loaded from config by using the static `HealthChecks.GetRunner` method.

Example MVC Controller:

```c#
public class HealthController : Controller
{
    public async Task<ContentResult> Index()
    {
        var healthCheckResponse = await HealthCheck.GetRunner().RunAsync();

        Response.StatusCode = healthCheckResponse.StatusCode;

        return Content(healthCheckResponse.Serialize(true), healthCheckResponse.ContentType);
    }
}
```

Example appsettings.json:

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
