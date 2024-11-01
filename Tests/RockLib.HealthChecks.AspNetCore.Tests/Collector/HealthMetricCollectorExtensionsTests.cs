using RockLib.HealthChecks.AspNetCore.Collector;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests.Collector;

public class HealthMetricCollectorExtensionsTests
{
    [Fact]
    public void GetCountReturnsArraySizeWhenAllTrue()
    {
        var collector = new HealthMetricCollector(3);
        Assert.Equal(3, collector.GetCount(_ => true));
    }
    [Fact]
    public void GetCountReturnsCorrectCount()
    {
        var collector = new HealthMetricCollector(3);
        collector.Collect(1);
        collector.Collect(2);
        collector.Collect(3);
        Assert.Equal(2, collector.GetCount(x => x > 1));
    }
}