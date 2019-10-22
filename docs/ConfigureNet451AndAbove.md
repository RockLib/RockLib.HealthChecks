# How to configure for .NET Framework 4.5.1 and above

Applications and libraries referencing RockLib.HealthChecks and targeting .NET Framework 4.5.1 and above can be configured with `app.config` or `web.config` (though they can still be configured with appsetting.json or any other configuration provider). The static `HealthCheck.Runners` property is defined by default by the "RockLib.HealthChecks" sub-section of the `Config.Root` property from the `RockLib.Configuration` (from the `RockLib.Configuration` package).

Like any custom configuration section, it must be declared in the `<configSections>` element, as follows. The rest of the examples in this document assume that the `RockLib.HealthChecks` section has been declared as follows.

```xml
<configuration>
  <configSections>
    <section name="RockLib.HealthChecks" type="RockLib.Configuration.RockLibConfigurationSection, RockLib.Configuration" />
  </configSections>
</configuration>
```

---

In this example, a health check runner is defined with one health check of type `ProcessUptimeHealthCheck`. Since no name is given, it is considered the default runner and can be retrieved by calling `HealthCheck.GetRunner()`.

```xml
<RockLib.HealthChecks>
  <Runner Description="Example health check">
    <HealthChecks type="RockLib.HealthChecks.System.ProcessUptimeHealthCheck, RockLib.HealthChecks" />
  </Runner>
</RockLib.HealthChecks>
```

---

This health check runner has more than one health check.

```xml
<RockLib.HealthChecks>
  <Runner Description="Another example health check">
    <HealthChecks type="RockLib.HealthChecks.System.DiskDriveHealthCheck, RockLib.HealthChecks">
      <value WarnGigabytes="30"
             FailGigabytes="5"/>
    </HealthChecks>
    <HealthChecks type="RockLib.HealthChecks.System.SystemUptimeHealthCheck, RockLib.HealthChecks" />
  </Runner>
</RockLib.HealthChecks>
```

---

This example has multiple runners defined. The first one can be retrieved by calling `HealthCheck.GetRunner("EmptyRunner")` and the second one (since it is the default runner) by calling `HealthCheck.GetRunner()`.

```xml
<RockLib.HealthChecks>
  <Runner Name="EmptyRunner"
          Description="Empty health check runner, always passes." />
  <Runner Description="Full health check runner">
    <HealthChecks type="RockLib.HealthChecks.System.ProcessUptimeHealthCheck, RockLib.HealthChecks" />
    <HealthChecks type="RockLib.HealthChecks.System.DiskDriveHealthCheck, RockLib.HealthChecks" />
    <HealthChecks type="RockLib.HealthChecks.System.SystemUptimeHealthCheck, RockLib.HealthChecks" />
  </Runner>
</RockLib.HealthChecks>
```
