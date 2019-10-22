# How to format the output of Microsoft.Extensions.Diagnostics.HealthChecks in the draft RFC format

To format the output of Microsoft.Extensions.Diagnostics.HealthChecks in the draft RFC format, use the RockLib.HealthChecks.AspNetCore.ResponseWriter nuget package. This package adds the `RockLibHealthChecks.ResponseWriter` that can be set as the `Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions.ResponseWriter`. This will map the `HealthReport` into a `RockLib.HealthChecks.HealthResponse`.

```c#
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = RockLibHealthChecks.ResponseWriter
        });
    }
}
```

This is generally set up in the Program.cs.

```c#
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

Since the response writer doesn't have a way to pass in the usual RockLib options, configuration is handled with static parameters on the `RockLibHealthChecks` class.

Property               | Default Value      | Description
---------------------- | ------------------ | -----------
Indent                 | `false`            | Gets or sets a value indicating whether the `ResponseWriter` delegate will indent its JSON output.

This can be done in the 'Startup.cs'

```c#
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        RockLibHealthChecks.Indent = true;

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = RockLibHealthChecks.ResponseWriter
        });
    }
}
```