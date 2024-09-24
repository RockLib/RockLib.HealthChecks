using Microsoft.Extensions.DependencyInjection;
using RockLib.HealthChecks.Collector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.System;

/// <summary>
/// 
/// </summary>
public class MetricsHealthCheck : IHealthCheck
{
    private readonly IHealthMetricCollectorFactory _collectorFactory;
    private readonly double _defaultWarningThreshold;
    private readonly double _defaultErrorThreshold;
    private readonly CollectorOptions[] _collectors;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="warningThreshold"></param>
    /// <param name="errorThreshold"></param>
    /// <param name="samples"></param>
    /// <param name="collectors"></param>
    public MetricsHealthCheck(IServiceProvider serviceProvider, double? warningThreshold = null,
        double? errorThreshold = null, int? samples = null, CollectorOptions[]? collectors = null)
    {
        _collectorFactory = serviceProvider.GetRequiredService<IHealthMetricCollectorFactory>();
        _defaultWarningThreshold = warningThreshold ?? .9;
        _defaultErrorThreshold = errorThreshold ?? .75;

        _collectors = collectors ?? [];
        foreach (var collector in _collectors)
        {
            _collectorFactory.ConfigureCollector(collector.Name, collector.Samples ?? samples);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<HealthCheckResult>> CheckAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<HealthCheckResult>();

        foreach (var kvPair in _collectorFactory.GetCollectors())
        {
            var name = kvPair.Key;
            var collector = kvPair.Value;
            var result = new HealthCheckResult
            {
                ComponentName = ComponentName
            };

            // collect the metrics
            var successCnt = collector.GetCount(cd => cd is > 199 and < 300);
            var redirectCnt = collector.GetCount(cd => cd is > 299 and < 400);
            var total = collector.GetCount(x => x > 0);
            result.Add("host", name);
            result.Add("http_2xx", successCnt);
            result.Add("http_3xx", redirectCnt);
            result.Add("http_4xx", collector.GetCount(cd => cd is > 399 and < 500));
            result.Add("http_5xx", collector.GetCount(cd => cd > 499));

            // compute the outcome
            var (warnThreshold, errorThreshold) = GetThresholds(name);
            var rate = total > 0 ? (successCnt + redirectCnt) / total : 1;
            HealthStatus? status = rate > warnThreshold ? HealthStatus.Pass : null;
            status ??= rate > errorThreshold ? HealthStatus.Warn : HealthStatus.Fail;
            result.Status = status;

            results.Add(result);
        }

        return await Task.FromResult(results).ConfigureAwait(false);
    }

    private (double, double) GetThresholds(string name)
    {
        var options = Array.Find(_collectors, opts =>
            string.Equals(opts.Name, name, StringComparison.OrdinalIgnoreCase));
        var warn = options?.WarningThreshold ?? _defaultWarningThreshold;
        var error = options?.ErrorThreshold ?? _defaultErrorThreshold;
        return (warn, error);
    }

    /// <summary>
    /// 
    /// </summary>
    public string? ComponentName { get; } = Environment.MachineName;

    /// <summary>
    /// 
    /// </summary>
    public string? MeasurementName { get; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string? ComponentType { get; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string? ComponentId { get; } = string.Empty;
}