namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// 
/// </summary>
public record CollectorOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public int? Samples { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? WarningThreshold { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? ErrorThreshold { get; set; }
}