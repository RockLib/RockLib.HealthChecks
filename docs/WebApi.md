# How to RockLib.HealthChecks to an ASP.NET WebApi application

To add a health endpoint to an ASP.NET WebApi application, use the RockLib.HealthChecks.WebApi nuget package. This package adds the `MapHealthRoute` extension method on the `HttpRoutesCollection` used in the `WebApiConfig` class.

```c#
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.Routes.MapHealthRoute();
    }
}
```

This is generally set up in the Global.aspx.cs.

```c#
public class WebApiApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        GlobalConfiguration.Configure(WebApiConfig.Register);
    }
}
```

There are two versions of the `MapHealthRoute` method. One takes the name of the `HealthCheckRunner` that should be loaded from config and the other takes a `HealthCheckRunner` directly.

### Named HealthCheckRunner from config

Parameter              | Default Value      | Description
---------------------- | ------------------ | -----------
routes                 | Required           | The route collection.
healthCheckRunnerName  | `null`             | The name of the `IHealthCheckRunner` to use. If `null` or not provided, the default value of the `HealthCheck.GetRunner` method is used.
route                  | `/health`          | The route of the health endpoint.
indent                 | `false`            | Whether to indent the JSON output.


### Direct HealthCheckRunner

Parameter              | Default Value      | Description
---------------------- | ------------------ | -----------
routes                 | Required           | The route collection.
healthCheckRunner      | Required           | The `IHealthCheckRunner` to use. If `null`, the default value of the `HealthCheck.GetRunner` method is used.
route                  | `/health`          | The route of the health endpoint.
indent                 | `false`            | Whether to indent the JSON output.