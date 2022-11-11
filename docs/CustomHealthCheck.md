---
sidebar_position: 11
sidebar_label: 'Create custom health checks'
---

# How to create custom health checks

To create a custom health check, either implement the `IHealthCheck` interface or inherit from the `SingleResultHealthCheck` abstract class.

For custom health checks that generate a single result, inherit from `SingleResultHealthCheck`, override the `CheckAsync` method, and optionally specify parameters for the base constructor.

```csharp
public class MyHealthCheck : SingleResultHealthCheck
{
    public MyHealthCheck()
      : base(componentName: "my", measurementName: "healthCheck",
        componentType: "example", componentId: "2d458535-c9a8-45be-91c2-dfd30b7a093e")
    {
    }

    protected override Task CheckAsync(HealthCheckResult result, CancellationToken cancellationToken)
    {
        // TODO: Perform the actual health check, setting the Status property accordingly.
        result.Status = HealthStatus.Pass;
        return Task.CompletedTask;
    }
}
```

---

For custom health checks that generate multiple results, implement the `IHealthCheck` interface directly.

```csharp
public class AnotherHealthCheck : IHealthCheck
{
    public AnotherHealthCheck(string componentName = "another", string measurementName = "healthCheck",
        string componentType = "example", string componentId = "224ee0cd-e838-4ea9-95af-2bd31dc42850")
    {
        ComponentName = componentName;
        MeasurementName = measurementName;
        ComponentType = componentType;
        ComponentId = componentId;
    }

    public string ComponentName { get; }
    public string MeasurementName { get; }
    public string ComponentType { get; }
    public string ComponentId { get; }

    public async Task<IReadOnlyList<HealthCheckResult>> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        var results = new List<HealthCheckResult>();

        var result1 = this.CreateHealthCheckResult();
        result1.Status = HealthStatus.Pass;
        results.Add(result1);

        var result2 = this.CreateHealthCheckResult();
        result2.Status = HealthStatus.Warn;
        result2.Output = "This message should describe why the status is Warn.";
        results.Add(result2);

        return results;
    }
}
```

## .NET Framework 3.5 and 4.0

In .NET Framework 4.0 and below, there is no support for `async` methods, so the `IHealthCheck` interface and `SingleResultHealthCheck` abstract class have synchronous methods instead. Here is the signatures of the synchronous methods:

```csharp
public interface IHealthCheck
{
    IList<HealthCheckResult> Check();
}

public abstract class SingleResultHealthCheck : IHealthCheck
{
    protected abstract void Check(HealthCheckResult result);
}
```
