# How to RockLib.HealthChecks to an ASP.NET application via HttpModule

To add a health endpoint to an ASP.NET application via HttpModule, use the RockLib.HealthChecks.HttpModule nuget package. This package adds the `HealthCheckHttpModule` HTTP module. This can be wired up in the web.config.

```xml
<configuration>
  <system.webServer>
    <modules>
      <add name="HealthCheckHttpModule" type="RockLib.HealthChecks.HttpModule.HealthCheckHttpModule" preCondition="managedHandler" />
    </modules>
  </system.webServer>
</configuration>
```

Since the HTTP module doesn't have use a method to wire up, configuration is handled with static parameters on the `HealthCheckHttpModule` class.

Property               | Default Value      | Description
---------------------- | ------------------ | -----------
HealthCheckRunnerName  | `null`             | Gets or sets the name of the `IHealthCheckRunner`.
Route                  | `health`           | Gets or sets the route of the health endpoint.
Indent                 | `false`            | Gets or sets a value indicating whether to indent the JSON output.

This can be done in the composition root. One example is the 'Global.asax.cs'.

```c#
public class Global : System.Web.HttpApplication
{
    protected void Application_Start(object sender, EventArgs e)
    {
        HealthCheckHttpModule.HealthCheckRunnerName = "MyRunner";
        HealthCheckHttpModule.Route = "/CustomHealthRoute";
        HealthCheckHttpModule.Indent = true;

        // More startup stuff here
    }
}
```