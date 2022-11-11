---
sidebar_position: 3
sidebar_label: 'Add to an ASP.NET Core application'
---

# How to add RockLib.HealthChecks to an ASP.NET Core application

To add a health endpoint to an ASP.NET Core application, use the RockLib.HealthChecks.AspNetCore nuget package. This package adds the `UseRockLibHealthChecks` extension method on the `IApplicationBuilder` used in the `Startup` class.

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseRockLibHealthChecks();
    }
}
```

This is generally set up in the Program.cs.

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
}
```

There are two versions of the `UseRockLibHealthChecks` method. One takes the name of the `HealthCheckRunner` that should be loaded from config and the other takes a `HealthCheckRunner` directly.

## Named HealthCheckRunner from config

Parameter              | Default Value                     | Description
---------------------- | --------------------------------- | -----------
builder                | Required                          | The application builder.
healthCheckRunnerName  | `null`                            | The name of the `IHealthCheckRunner` to use. If `null` or not provided, the default value of the `HealthCheck.GetRunner` method is used.
route                  | `/health`                         | The route of the health endpoint.
formatter              | `NewtonsoftJsonResponseFormatter` | The `IResponseFormatter` responsible for formatting health responses for the middleware's HTTP response body.


## Direct HealthCheckRunner

Parameter              | Default Value                     | Description
---------------------- | --------------------------------- | -----------
builder                | Required                          | The application builder.
healthCheckRunner      | Required                          | The `IHealthCheckRunner` to use. If `null`, the default value of the `HealthCheck.GetRunner` method is used.
route                  | `/health`                         | The route of the health endpoint.
formatter              | `NewtonsoftJsonResponseFormatter` | The `IResponseFormatter` responsible for formatting health responses for the middleware's HTTP response body.
