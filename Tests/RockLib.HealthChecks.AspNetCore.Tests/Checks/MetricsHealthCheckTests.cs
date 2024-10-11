using Microsoft.Extensions.DependencyInjection;
using RockLib.HealthChecks.AspNetCore.Checks;
using RockLib.HealthChecks.AspNetCore.Collector;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests.Checks;

public class MetricsHealthCheckTests
{
    private readonly MetricsHealthCheck _healthCheck;
    private readonly HealthMetricCollectorFactory _collectorFactory;

    public MetricsHealthCheckTests()
    {
        _collectorFactory = new HealthMetricCollectorFactory();

        var services = new ServiceCollection();
        services.AddSingleton<IHealthMetricCollectorFactory>(_collectorFactory);
        var serviceProvider = services.BuildServiceProvider();
        var collectors = new[]
        {
            new CollectorOptions { Name = "test", Samples = 10 }
        };
        _healthCheck = new MetricsHealthCheck(serviceProvider, .75, .5, 10, collectors);
    }

    [Fact]
    public void MetricsHealthCheckThrowsIfMissingDependency()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => new MetricsHealthCheck(serviceProvider));
    }

    [Fact]
    public void MetricsHealthCheckInitializesWithDefaultArgs()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IHealthMetricCollectorFactory, HealthMetricCollectorFactory>();
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(new MetricsHealthCheck(serviceProvider));
    }

    [Fact]
    public async Task CheckAsyncComputesPassWhenNoDataCollected()
    {
        var results = await _healthCheck.CheckAsync();

        Assert.Single(results);
        Assert.Equal(Environment.MachineName, results[0].ComponentName);
        Assert.Equal("test", results[0]["host"]);
        Assert.Equal(0, results[0]["http_2xx"]);
        Assert.Equal(0, results[0]["http_4xx"]);
        Assert.Equal(0, results[0]["http_5xx"]);
        Assert.Equal(HealthStatus.Pass, results[0]["status"]);
    }

    [Fact]
    public async Task CheckAsyncComputesPass()
    {
        // seed some collected data
        var collector = _collectorFactory.LeaseCollector("test");
        collector.Collect(200);
        collector.Collect(200);
        collector.Collect(200);
        collector.Collect(204);
        collector.Collect(403);

        var results = await _healthCheck.CheckAsync();

        Assert.Single(results);
        Assert.Equal(Environment.MachineName, results[0].ComponentName);
        Assert.Equal("test", results[0]["host"]);
        Assert.Equal(4, results[0]["http_2xx"]);
        Assert.Equal(1, results[0]["http_4xx"]);
        Assert.Equal(0, results[0]["http_5xx"]);
        Assert.Equal(HealthStatus.Pass, results[0]["status"]);
    }

    [Fact]
    public async Task CheckAsyncComputesFail()
    {
        // seed some collected data
        var collector = _collectorFactory.LeaseCollector("test");
        collector.Collect(200);
        collector.Collect(200);
        collector.Collect(204);
        collector.Collect(400);
        collector.Collect(400);
        collector.Collect(404);
        collector.Collect(500);
        collector.Collect(502);

        var results = await _healthCheck.CheckAsync();

        Assert.Single(results);
        Assert.Equal(Environment.MachineName, results[0].ComponentName);
        Assert.Equal("test", results[0]["host"]);
        Assert.Equal(3, results[0]["http_2xx"]);
        Assert.Equal(3, results[0]["http_4xx"]);
        Assert.Equal(2, results[0]["http_5xx"]);
        Assert.Equal(HealthStatus.Fail, results[0]["status"]);
    }

    [Fact]
    public async Task CheckAsyncComputesWarning()
    {
        // seed some collected data
        var collector = _collectorFactory.LeaseCollector("test");
        collector.Collect(200);
        collector.Collect(200);
        collector.Collect(200);
        collector.Collect(500);

        var results = await _healthCheck.CheckAsync();

        Assert.Single(results);
        Assert.Equal(Environment.MachineName, results[0].ComponentName);
        Assert.Equal("test", results[0]["host"]);
        Assert.Equal(3, results[0]["http_2xx"]);
        Assert.Equal(0, results[0]["http_4xx"]);
        Assert.Equal(1, results[0]["http_5xx"]);
        Assert.Equal(HealthStatus.Warn, results[0]["status"]);
    }
}