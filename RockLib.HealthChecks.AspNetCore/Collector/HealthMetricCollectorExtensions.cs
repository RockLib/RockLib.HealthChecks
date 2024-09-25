using System;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// IHealthMetricCollector extensions.
/// </summary>
public static class HealthMetricCollectorExtensions
{
    /// <summary>
    /// Return a count of the metrics that match the predicate.
    /// </summary>
    /// <param name="collector"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int GetCount(this IHealthMetricCollector collector, Func<int, bool> predicate)
    {
        return collector?.GetMetrics(predicate).Length ?? 0;
    }
}