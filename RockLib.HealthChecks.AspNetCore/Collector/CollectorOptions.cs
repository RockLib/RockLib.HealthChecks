namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// Options for configuring a <see cref="HealthMetricCollector"/>.
/// </summary>
/// <remarks>Intended to be reused in other IMetricCollector implementations.</remarks>
public record CollectorOptions
{
    /// <summary>
    /// The name of the collector.  Defaults to <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>Use http host name (e.g. "myapp.foc.zone") if collecting http client response codes.</remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// number of samples to keep in the collector.  Defaults to 100.
    /// </summary>
    public int? Samples { get; set; }

    /// <summary>
    /// warning threshold for the collector.  Defaults to 0.9.
    /// </summary>
    public double? WarningThreshold { get; set; }

    /// <summary>
    /// error threshold for the collector.  Defaults to 0.75.
    /// </summary>
    public double? ErrorThreshold { get; set; }
}