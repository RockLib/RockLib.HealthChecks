---
sidebar_position: 2
sidebar_label: 'Metrics (HTTP) Health Check'
---

# MetricsHealthCheck

This health check monitors actual (live) HTTP calls to downstream systems to collect response status codes, and reports their outcomes. If the number of failed calls (StatusCode >= 400) is 
below `WarnThreshold`, results will have a status of `Warn`. If below `FailThreshold`, results will have a status of `Fail`.


In your Startup.cs file, add the following code to `ConfigureServices`:
```csharp
builder.ConfigureRockLibHealthChecks();
```
The above is in addition to `app.UseRockLibHealthChecks();`, in the `Configure` method.


Parameter          | Default Value | Description
------------------ |---------------| -----------
warningThreshold   | `"0.9"`       | The lowest percentage of HTTP status codes >=400, below which results in a `Warn` status.
errorThreshold     | `"0.75"`      | The lowest percentage of HTTP status codes >=400, below which results in a `Fail` status.
samples            | `"100"`       | The number of sample data points to keep.  Collection is done using a sliding window - e.g. "the 100 most recent".
collectors         | `"[]"`        | An explicit list of collector thresholds.  If not provided, the default values (above) will be used.
collector->Name    | `""`          | The http host name of the downstream system (to bind the settings to).

Note: A collector is created for each http host name regardless.  The above settings are only necessary if you want to override the default sample size or thresholds.


Example appsettings.json:

```json
{
  "RockLib.HealthChecks": {
      "healthChecks": [
        {
          "type": "RockLib.HealthChecks.AspNetCore.Checks.MetricsHealthCheck, RockLib.HealthChecks.AspNetCore",
          "value": {
            "warningThreshold": ".90",
            "errorThreshold": ".85",
            "samples": "100",
            "collectors": [
              {
                "Name": "www.google.com",
                "samples": "50",
                "warningThreshold": ".09",
                "errorThreshold": ".05"
              }
            ]
          }
        }
      ]
    }
}
```
