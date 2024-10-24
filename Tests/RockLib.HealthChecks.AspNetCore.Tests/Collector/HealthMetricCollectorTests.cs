using RockLib.HealthChecks.AspNetCore.Collector;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests.Collector;

public class HealthMetricCollectorTests
{
    [Fact]
    public void HealthMetricCollectorCollects()
    {
        var collector = new HealthMetricCollector(3);
        collector.Collect(1);
        collector.Collect(2);
        collector.Collect(3);
        var arr = new[] { 1, 2, 3 };
        Assert.Equal(arr, collector.GetMetrics());

        // begin overwriting
        collector.Collect(4);
        collector.Collect(5);
        arr = [4, 5, 3];
        Assert.Equal(arr, collector.GetMetrics());
    }

    [Fact]
    public void HealthMetricCollectorGetMetricsReturnsOnlyMatches()
    {
        var collector = new HealthMetricCollector(3);
        collector.Collect(1);
        collector.Collect(2);
        collector.Collect(3);
        var arr = new[] { 3 };
        Assert.Equal(arr, collector.GetMetrics(x => x > 2));
    }

    [Fact]
    public void HealthMetricCollectorGetMetricsReturnsEmptyIfNoMatch()
    {
        var collector = new HealthMetricCollector(3);
        Assert.Equal([], collector.GetMetrics(x => x == 4));
    }
}