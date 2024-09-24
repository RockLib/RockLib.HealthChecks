using System.Collections.Generic;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// 
/// </summary>
public interface IHealthMetricCollectorFactory
{
    internal void ConfigureCollector(string name, int? samples);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IHealthMetricCollector LeaseCollector(string? name);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Dictionary<string, IHealthMetricCollector> GetCollectors();
}

/// <summary>
/// 
/// </summary>
public class HealthMetricCollectorFactory : IHealthMetricCollectorFactory
{
    private readonly Dictionary<string, IHealthMetricCollector> _collectors;
    private const int DefaultSamples = 100;

    /// <summary>
    /// 
    /// </summary>
    public HealthMetricCollectorFactory()
    {
        _collectors = new Dictionary<string, IHealthMetricCollector>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IHealthMetricCollector LeaseCollector(string? name)
    {
        name ??= string.Empty;
        if (_collectors.TryGetValue(name, out var value)) return value;

        value = new HealthMetricCollector(DefaultSamples);
        _collectors[name] = value;
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, IHealthMetricCollector> GetCollectors()
    {
        return _collectors;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="samples"></param>
    public void ConfigureCollector(string name, int? samples)
    {
        _collectors[name] = new HealthMetricCollector(samples ?? DefaultSamples);
    }
}