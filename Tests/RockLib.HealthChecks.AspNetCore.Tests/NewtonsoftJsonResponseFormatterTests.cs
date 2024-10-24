using Newtonsoft.Json;
using System;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests;

public static class NewtonsoftJsonResponseFormatterTests
{
    [Fact]
    public static void CreateWithNoResults()
    {
        var formatter = new NewtonsoftJsonResponseFormatter();
        var result = formatter.Format(new HealthResponse());

        Assert.Equal("{\"status\":\"pass\"}", result);
    }

    [Fact]
    public static void CreateWithResults()
    {
        var formatter = new NewtonsoftJsonResponseFormatter();
        var result = formatter.Format(
            new HealthResponse(new[]
            {
                new HealthCheckResult { Status = HealthStatus.Pass },
                new HealthCheckResult { Status = HealthStatus.Fail }
            }));

        Assert.Equal("{\"status\":\"fail\",\"checks\":{\"\":[{\"status\":\"pass\"},{\"status\":\"fail\"}]}}", result);
    }

    [Fact]
    public static void CreateWithFormatting()
    {
        var formatter = new NewtonsoftJsonResponseFormatter(Formatting.Indented);
        var result = formatter.Format(new HealthResponse());

        Assert.Equal($"{{{Environment.NewLine}  \"status\": \"pass\"{Environment.NewLine}}}", result);
    }

    [Fact]
    public static void CreateWithSettings()
    {
#pragma warning disable CA2326 // Do not use TypeNameHandling values other than None
        var formatter = new NewtonsoftJsonResponseFormatter(settings: new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
#pragma warning restore CA2326 // Do not use TypeNameHandling values other than None
        var result = formatter.Format(new HealthResponse());

        Assert.Equal("{\"$type\":\"RockLib.HealthChecks.HealthResponse, RockLib.HealthChecks\",\"status\":\"pass\"}", result);
    }
}
