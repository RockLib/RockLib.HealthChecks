---
sidebar_position: 8
sidebar_label: 'Configure with appsettings.json'
---

# How to configure with appsettings.json

Applications and libraries referencing RockLib.HealthChecks and targeting .NET Core, .NET Standard, or .NET Framework 4.5.1 and above can be configured with `appsettings.json`. The static `HealthCheck.Runners` property is defined by default by the "RockLib.HealthChecks" sub-section of the `Config.Root` property (from the `RockLib.Configuration` package).

---

In this example, a health check runner is defined with one health check of type `ProcessUptimeHealthCheck`. Since no name is given, it is considered the default runner and can be retrieved by calling `HealthCheck.GetRunner()`.

```json
{
  "RockLib.HealthChecks": {
    "Description": "Example health check",
    "HealthChecks": { "type": "RockLib.HealthChecks.System.ProcessUptimeHealthCheck, RockLib.HealthChecks" }
  }
}
```

---

This health check runner has more than one health check.

```json
{
  "RockLib.HealthChecks": {
    "Description": "Another example health check",
    "HealthChecks": [
      {
        "type": "RockLib.HealthChecks.System.DiskDriveHealthCheck, RockLib.HealthChecks",
        "value": {
          "WarnGigabytes": "30",
          "FailGigabytes": "5"
        }
      },
      { "type": "RockLib.HealthChecks.System.SystemUptimeHealthCheck, RockLib.HealthChecks" }
    ]
  }
}
```

---

This example has multiple runners defined. The first one can be retrieved by calling `HealthCheck.GetRunner("EmptyRunner")` and the second one (since it is the default runner) by calling `HealthCheck.GetRunner()`.

```json
{
  "RockLib.HealthChecks": [
    {
      "Name": "EmptyRunner",
      "Description": "Empty health check runner, always passes."
    },
    {
      "Description": "Full health check runner",
      "HealthChecks": [
        { "type": "RockLib.HealthChecks.System.ProcessUptimeHealthCheck, RockLib.HealthChecks" }
        { "type": "RockLib.HealthChecks.System.DiskDriveHealthCheck, RockLib.HealthChecks" }
        { "type": "RockLib.HealthChecks.System.SystemUptimeHealthCheck, RockLib.HealthChecks" }
      ]
    }
  ]
}
```
