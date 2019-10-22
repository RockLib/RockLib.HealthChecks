# How to configure for .NET Framework 4.5 and below

Applications and libraries referencing RockLib.HealthChecks and targeting .NET Framework 4.5.1 and above can be configured with `app.config` or `web.config`. The static `HealthCheck.Runners` property is defined by default by the "rockLib.healthChecks" section from `ConfigurationManager` as type `RockLibHealthChecksSection` by calling its `CreateRunners` method.

Like any custom configuration section, it must be declared in the `<configSections>` element, as follows. The rest of the examples in this document assume that the `rockLib.healthChecks` section has been declared as follows.

```xml
<configuration>
  <configSections>
    <section name="rockLib.healthChecks" type="RockLib.HealthChecks.Configuration.RockLibHealthChecksSection, RockLib.HealthChecks" />
  </configSections>
</configuration>
```

---

In this example, a health check runner is defined with one health check of type `ProcessUptimeHealthCheck`. Since no name is given, it is considered the default runner and can be retrieved by calling `HealthCheck.GetRunner()`.

```xml
<rockLib.healthChecks>
  <runners>
    <runner description="Example health check">
      <healthChecks>
        <healthCheck type="RockLib.HealthChecks.System.ProcessUptimeHealthCheck, RockLib.HealthChecks" />
      </healthChecks>
    </runner>
  </runners>
</rockLib.healthChecks>
```

---

This health check runner has more than one health check.

```xml
<rockLib.healthChecks>
  <runners>
    <runner description="Another example health check">
      <healthChecks>
        <healthCheck type="RockLib.HealthChecks.System.DiskDriveHealthCheck, RockLib.HealthChecks"
                     warnGigabytes="30"
                     failGigabytes="5" />
        <healthCheck type="RockLib.HealthChecks.System.SystemUptimeHealthCheck, RockLib.HealthChecks" />
      </healthChecks>
    </runner>
  </runners>
</rockLib.healthChecks>
```

---

This example has multiple runners defined. The first one can be retrieved by calling `HealthCheck.GetRunner("EmptyRunner")` and the second one (since it is the default runner) by calling `HealthCheck.GetRunner()`.

```xml
<rockLib.healthChecks>
  <runners>
    <runner name="EmptyRunner"
            description="Empty health check runner, always passes." />
    <runner description="Full health check runner">
      <healthChecks>
        <healthCheck type="RockLib.HealthChecks.System.ProcessUptimeHealthCheck, RockLib.HealthChecks" />
        <healthCheck type="RockLib.HealthChecks.System.DiskDriveHealthCheck, RockLib.HealthChecks" />
        <healthCheck type="RockLib.HealthChecks.System.SystemUptimeHealthCheck, RockLib.HealthChecks" />
      </healthChecks>
    </runner>
  </runners>
</rockLib.healthChecks>
```
