using System;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// 
/// </summary>
public static class HealthMetricCollectorExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collector"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int GetCount(this IHealthMetricCollector collector, Func<int, bool> predicate)
    {
        return collector?.GetMetrics(predicate).Length ?? 0;
    }
}