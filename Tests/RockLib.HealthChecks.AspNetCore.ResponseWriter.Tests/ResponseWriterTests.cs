using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.ResponseWriter.Tests;

// Since we're testing static members on a static class,
// we shouldn't run them in parallel.
public static class ResponseWriterTests
{
    [Theory]
    [AutoData]
    public static async Task ResponseWriterHideExceptionsDefaultShouldShowExceptionsProperty(
        HealthReport healthReport)
    {
        // Arrange
        var httpContext = CreateHttpContext();

        // Act
        await RockLibHealthChecks.ResponseWriter(httpContext, healthReport).ConfigureAwait(true);

        // Assert
        var jsonBody = await GetJsonBody(httpContext).ConfigureAwait(true);
        var healthCheckEntries = GetHealthCheckEntries(jsonBody);

        healthCheckEntries
            .Any(x => x["exception"] != null)
            .Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    public static async Task ResponseWriterHideExceptionsSetToFalseShouldShowExceptionsProperty(
        HealthReport healthReport)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        RockLibHealthChecks.HideExceptions = false;

        // Act
        await RockLibHealthChecks.ResponseWriter(httpContext, healthReport).ConfigureAwait(true);

        // Assert
        var jsonBody = await GetJsonBody(httpContext).ConfigureAwait(true);
        var healthCheckEntries = GetHealthCheckEntries(jsonBody);

        healthCheckEntries
            .Any(x => x["exception"] != null)
            .Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    public static async Task ResponseWriterHideExceptionsSetToTrueShouldHideExceptionsProperty(
        HealthReport healthReport)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        RockLibHealthChecks.HideExceptions = true;

        // Act
        await RockLibHealthChecks.ResponseWriter(httpContext, healthReport).ConfigureAwait(true);

        // Assert
        var jsonBody = await GetJsonBody(httpContext).ConfigureAwait(true);
        var healthCheckEntries = GetHealthCheckEntries(jsonBody);

        healthCheckEntries
            .Any(x => x["exception"] != null)
            .Should()
            .BeFalse();
    }

    [Theory]
    [AutoData]
    public static async Task ResponseWriterHideOutputsDefaultShouldShowOutputProperty(
        HealthReport healthReport)
    {
        // Arrange
        var httpContext = CreateHttpContext();

        // Act
        await RockLibHealthChecks.ResponseWriter(httpContext, healthReport).ConfigureAwait(true);

        // Assert
        var jsonBody = await GetJsonBody(httpContext).ConfigureAwait(true);
        var healthCheckEntries = GetHealthCheckEntries(jsonBody);

        healthCheckEntries
            .Any(x => x["output"] != null)
            .Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    public static async Task ResponseWriterHideOutputsSetToFalseShouldShowOutputProperty(
        HealthReport healthReport)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        RockLibHealthChecks.HideOutputs = false;

        // Act
        await RockLibHealthChecks.ResponseWriter(httpContext, healthReport).ConfigureAwait(true);

        // Assert
        var jsonBody = await GetJsonBody(httpContext).ConfigureAwait(true);
        var healthCheckEntries = GetHealthCheckEntries(jsonBody);

        healthCheckEntries
            .Any(x => x["output"] != null)
            .Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    public static async Task ResponseWriterHideOutputsSetToTrueShouldHideOutputProperty(
        HealthReport healthReport)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        RockLibHealthChecks.HideOutputs = true;

        // Act
        await RockLibHealthChecks.ResponseWriter(httpContext, healthReport).ConfigureAwait(true);

        // Assert
        var jsonBody = await GetJsonBody(httpContext).ConfigureAwait(true);
        var healthCheckEntries = GetHealthCheckEntries(jsonBody);

        healthCheckEntries
            .Any(x => x["output"] != null)
            .Should()
            .BeFalse();
    }

    private static IEnumerable<JToken> GetHealthCheckEntries(string jsonBody)
    {
        return JObject.Parse(jsonBody)!
            .SelectToken("checks")!
            .Children()
            .SelectMany(x => x.First!);
    }

    private static async Task<string> GetJsonBody(DefaultHttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(httpContext.Response.Body);
        var bodyStr = await reader.ReadToEndAsync().ConfigureAwait(false);

        return bodyStr;
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext {
            Response = {
                Body = new MemoryStream()
            }
        };
        return httpContext;
    }
}