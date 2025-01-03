﻿using System.Collections.Generic;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// Interface definition for the container of collectors.  We use this to manage the lifecycle of collectors.
/// </summary>
/// <remarks>factory is Singleton at runtime</remarks>
/// <!-- This should be moved to a separate file when additional implementations are added. -->
public interface IHealthMetricCollectorFactory
{
    /// <summary>
    /// used by the health check middleware to right-size collectors (by name)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="samples"></param>
    internal void ConfigureCollector(string name, int? samples = null);

    /// <summary>
    /// Returns a handle to a collector by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IHealthMetricCollector LeaseCollector(string? name);

    /// <summary>
    /// Returns all collectors held by the factory
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, IHealthMetricCollector> GetCollectors();

    /// <summary>
    /// Override the default sample size for new collectors
    /// </summary>
    /// <param name="samples"></param>
    public void SetDefaultSampleSize(int? samples);
}

/// <summary>
/// A factory for accessing <see cref="IHealthMetricCollector"/> instances.  The factory holds the collectors and manages their lifecycle.
/// </summary>
public class HealthMetricCollectorFactory : IHealthMetricCollectorFactory
{
    private readonly Dictionary<string, IHealthMetricCollector> _collectors;
    private int _defaultSamples = 100;

    /// <summary>
    /// Default constructor - typically used by the DI container
    /// </summary>
    public HealthMetricCollectorFactory()
    {
        _collectors = new Dictionary<string, IHealthMetricCollector>();
    }

    /// <summary>
    /// Obtain a collector by name.
    /// If the collector does not exist it's created (w/size=100).
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IHealthMetricCollector LeaseCollector(string? name)
    {
        name ??= string.Empty;
        if (_collectors.TryGetValue(name, out var value)) return value;

        value = new HealthMetricCollector(_defaultSamples);
        _collectors[name] = value;
        return value;
    }

    /// <summary>
    /// Exposes all collectors held by the factory
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, IHealthMetricCollector> GetCollectors()
    {
        return _collectors;
    }

    /// <summary>
    /// Set the default sample size for all collectors (w/o explicit size).
    /// </summary>
    /// <param name="samples"></param>
    public void SetDefaultSampleSize(int? samples)
    {
        _defaultSamples = samples ?? _defaultSamples;
    }

    /// <summary>
    /// Configure a collector by name with the given sample size.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="samples"></param>
    public void ConfigureCollector(string name, int? samples = null)
    {
        _collectors[name] = new HealthMetricCollector(samples ?? _defaultSamples);
    }
}