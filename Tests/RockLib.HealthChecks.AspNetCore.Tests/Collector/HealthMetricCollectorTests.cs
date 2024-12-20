using RockLib.HealthChecks.AspNetCore.Collector;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        var arr = new[] { 3, 1, 2 };
        Assert.Equal(arr, collector.GetMetrics());

        // begin overwriting
        collector.Collect(4);
        collector.Collect(5);
        arr = [3, 4, 5];
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

    /// <summary>
    /// This test is the antithesis of the next test.  It proves that the collector requires thread safety.
    /// If this test ever starts failing we may no longer need to use Interlocked.Increment in the Collect method.
    /// </summary>
    [Fact]
    public void ProofCollectorRequiresThreadSafety()
    {
        var collector = new HealthMetricCollector(3);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var cnt = 0;
        var res = Parallel.For(0, 10, _ =>
        {
            while (stopwatch.ElapsedMilliseconds < 1000)
            {
                collector.Collect(1);
                ++cnt; // unsafe; being accessed by multiple threads at once
            }
        });
        stopwatch.Stop();
        Assert.True(res.IsCompleted);
        Assert.True(cnt < collector.GetImpressionCount());
    }

    [Fact]
    public void HealthMetricCollectorIsThreadSafe()
    {
        var collector = new HealthMetricCollector(3);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var cnt = 0;
        var res = Parallel.For(0, 100, _ =>
        {
            while (stopwatch.ElapsedMilliseconds < 2500)
            {
                collector.Collect(1);
                Interlocked.Increment(ref cnt); // safe; leverages a locking mechanism
            }
        });
        stopwatch.Stop();
        Assert.True(res.IsCompleted);
        Assert.Equal(collector.GetImpressionCount(), cnt);
    }
}