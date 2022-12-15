using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSHealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;

namespace RockLib.HealthChecks.AspNetCore.ResponseWriter;

/// <summary>
/// Defines a delegate that maps its <see cref="HealthReport"/> parameter to the <see cref="HealthResponse"/>
/// type and writes it to its <see cref="HttpContext.Response"/> parameter. Intended to be used to set the value
/// of the <see cref="HealthCheckOptions.ResponseWriter"/> property.
/// </summary>
public static class RockLibHealthChecks
{
    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResponseWriter"/> delegate will indent
    /// its JSON output.
    /// </summary>
    public static bool Indent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResponseWriter"/> delegate will display
    /// the exception property in its JSON output.
    /// The default is to show the exception.
    /// </summary>
    public static bool HideExceptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResponseWriter"/> delegate will hide
    /// the output property in its JSON output.
    /// The default is to show the output.
    /// </summary>
    public static bool HideOutputs { get; set; }

    /// <summary>
    /// Gets a delegate that maps its <see cref="HealthReport"/> parameter to the <see cref="HealthResponse"/>
    /// type and writes it to its <see cref="HttpContext.Response"/> parameter. Intended to be used to set the
    /// value of the <see cref="HealthCheckOptions.ResponseWriter"/> property.
    /// </summary>
    public static Func<HttpContext, HealthReport, Task> ResponseWriter { get; } = WriteResponse;

    private static async Task WriteResponse(HttpContext httpContext, HealthReport healthReport)
    {
        var results = healthReport.Entries.Select(x =>
        {
            var entry = x.Value;

            var result = new HealthCheckResult
            {
                ComponentName = GetComponentName(x.Key),
                MeasurementName = GetMeasurementName(x.Key),
                Status = MapStatus(entry.Status),
                ["duration"] = entry.Duration
            };

            if (!HideExceptions)
            {
                result["exception"] = entry.Exception?.ToString();
            }

            if (!HideOutputs)
            {
                result.Output = entry.Description;
            }

            // TODO: Detect collisions between entry.Exception/result.Output,
            // entry.Description/result["description", or entry.Duration/result["duration"].

            foreach (var data in entry.Data)
            {
                result[data.Key] = data.Value;
            }

            return result;
        }).ToList();

        var response = new HealthResponse(results)
        {
            Status = MapStatus(healthReport.Status),
            Notes = new List<string> { $"TotalDuration: {healthReport.TotalDuration}" }
        };

        httpContext.Response.ContentType = response.ContentType;
        await httpContext.Response.WriteAsync(response.Serialize(Indent)).ConfigureAwait(false);
    }

    private static HealthStatus MapStatus(MSHealthStatus status) => 
        status switch
        {
            MSHealthStatus.Unhealthy => HealthStatus.Fail,
            MSHealthStatus.Degraded => HealthStatus.Warn,
            MSHealthStatus.Healthy => HealthStatus.Pass,
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };

    private static string? GetComponentName(string key)
    {
        var split = key.Split(':');

        return split.Length switch
        {
            1 => null,
            _ => split[0],
        };
    }

    private static string GetMeasurementName(string key)
    {
        var split = key.Split(':');

        return split.Length switch
        {
            1 => split[0],
            2 => split[1],
            _ => string.Join("", split.Skip(1)),
        };
    }
}
