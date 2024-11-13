using RockLib.HealthChecks.AspNetCore.Collector;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests.Collector;

public class HealthMetricCollectorFactoryTests
{
    [Fact]
    public void ConfigureCollectorCreatesNewCollectorsDynamically()
    {
        var factory = new HealthMetricCollectorFactory();
        factory.ConfigureCollector("foo", 10);
        factory.ConfigureCollector("bar", 20);
        factory.ConfigureCollector("foo", 30); // overwrite
        factory.ConfigureCollector("baz");

        var collectors = factory.GetCollectors();
        Assert.Equal(3, collectors.Count);
        Assert.NotNull(collectors["foo"]);
        Assert.NotNull(collectors["bar"]);
        Assert.NotNull(collectors["baz"]);
    }

    [Fact]
    public void LeaseCollectorCreatesWithDefaultSize()
    {
        var factory = new HealthMetricCollectorFactory();
        var collector = factory.LeaseCollector(null!);
        Assert.NotNull(collector);
        Assert.Equal(100, collector.GetMetrics(_ => true).Length);
    }

    [Fact]
    public void LeaseCollectorSupportsSizeOverride()
    {
        var factory = new HealthMetricCollectorFactory();
        factory.SetDefaultSampleSize(50);
        var collector = factory.LeaseCollector(null!);
        Assert.NotNull(collector);
        Assert.Equal(50, collector.GetMetrics(_ => true).Length);
    }

    [Fact]
    public void LeaseCollectorSupportsSizeFallback()
    {
        var factory = new HealthMetricCollectorFactory();
        factory.SetDefaultSampleSize(null);
        var collector = factory.LeaseCollector(null!);
        Assert.NotNull(collector);
        Assert.Equal(100, collector.GetMetrics(_ => true).Length);
    }

    [Fact]
    public void LeaseCollectorReturnsTheSameInstanceByName()
    {
        var factory = new HealthMetricCollectorFactory();
        var collector1 = factory.LeaseCollector("foo");
        var collector2 = factory.LeaseCollector("foo");
        Assert.Same(collector1, collector2);
    }
}