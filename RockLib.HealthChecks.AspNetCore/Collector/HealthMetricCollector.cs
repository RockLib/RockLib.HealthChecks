using System;
using System.Linq;
using System.Threading;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// Interface definition for a health metric collectors.  "health metrics" not to be confused with "application metrics" (e.g. OTEL/Dynatrace).
/// </summary>
/// <!-- This should be moved to a separate file when additional implementations are added. -->
public interface IHealthMetricCollector
{
    /// <summary>
    /// Add a new metric to the collector.
    /// </summary>
    /// <param name="outcome"></param>
    void Collect(int outcome);

    /// <summary>
    /// Retrieve the metrics from the collector.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int[] GetMetrics(Func<int, bool>? predicate = null);
    
    ///<summary>
    /// Retrieve the number of impressions from the collector.
    /// </summary>
    long GetImpressionCount();
}

/// <summary>
/// A simple implementation of <see cref="IHealthMetricCollector"/> that stores metrics in a fixed-size array.
/// The fixed-size array is used as a circular buffer - "only store the N most-recent metrics".
/// </summary>
public class HealthMetricCollector : IHealthMetricCollector
{
    private long _impressionCount;
    private readonly int[] _metrics;
    private readonly int _size;

    /// <summary>
    /// Implementation of <see cref="IHealthMetricCollector"/> that stores integer metrics in a fixed-size array.
    /// </summary>
    /// <param name="size">how large to make the array</param>
    public HealthMetricCollector(int size)
    {
        _metrics = new int[size];
        _size = size;
    }

    /// <summary>
    /// see <see cref="IHealthMetricCollector.Collect(int)"/>
    /// </summary>
    /// <param name="outcome">the integer outcome to put into the array.</param>
    public void Collect(int outcome)
    {
        var idx = Interlocked.Increment(ref _impressionCount) % _size;
        _metrics[idx] = outcome;
    }

    /// <summary>
    /// see <see cref="IHealthMetricCollector.GetMetrics(Func{int, bool}?)"/>
    /// </summary>
    /// <param name="predicate">Func used to collate matching entries</param>
    /// <returns>an int[] of entries matching the predicate</returns>
    public int[] GetMetrics(Func<int, bool>? predicate = null)
    {
        return _metrics.Where(predicate ?? (x => x != 0)).ToArray();
    }

    /// <summary>
    /// see <see cref="IHealthMetricCollector.GetImpressionCount"/>
    /// </summary>
    /// <returns></returns>
    public long GetImpressionCount()
    {
        return _impressionCount;
    }
}