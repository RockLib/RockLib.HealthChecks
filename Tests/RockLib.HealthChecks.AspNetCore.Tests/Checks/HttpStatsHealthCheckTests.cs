using Microsoft.Extensions.DependencyInjection;
using RockLib.HealthChecks.AspNetCore.Checks;
using RockLib.HealthChecks.AspNetCore.Collector;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests.Checks;

public class HttpStatsHealthCheckTests
{
    private readonly HttpStatsHealthCheck _healthCheck;
    private readonly HealthMetricCollectorFactory _collectorFactory;

    public HttpStatsHealthCheckTests()
    {
        _collectorFactory = new HealthMetricCollectorFactory();

        var services = new ServiceCollection();
        services.AddSingleton<IHealthMetricCollectorFactory>(_collectorFactory);
        var serviceProvider = services.BuildServiceProvider();
        var collectors = new[]
        {
            new CollectorOptions { Name = "test", Samples = 10 }
        };
        _healthCheck = new HttpStatsHealthCheck(serviceProvider, .75, .5, 10, 0, collectors);
    }

    [Fact]
    public void HttpStatsHealthCheckThrowsIfMissingDependency()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => new HttpStatsHealthCheck(serviceProvider));
    }

    [Fact]
    public void HttpStatsHealthCheckInitializesWithDefaultArgs()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IHealthMetricCollectorFactory, HealthMetricCollectorFactory>();
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(new HttpStatsHealthCheck(serviceProvider));
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