using System;
using System.Linq;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// 
/// </summary>
public interface IHealthMetricCollector
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="outcome"></param>
    void Collect(int outcome);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int[] GetMetrics(Func<int, bool>? predicate = null);
}

/// <summary>
/// 
/// </summary>
public class HealthMetricCollector : IHealthMetricCollector
{
    private int _index;
    private readonly int[] _metrics;
    private readonly int _size;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    public HealthMetricCollector(int size)
    {
        _metrics = new int[size];
        _size = size;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outcome"></param>
    public void Collect(int outcome)
    {
        _metrics[_index++] = outcome;
        if (_index == _size) _index = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int[] GetMetrics(Func<int, bool>? predicate = null)
    {
        return _metrics.Where(predicate ?? (x => x != 0)).ToArray();
    }
}