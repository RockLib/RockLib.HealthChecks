using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.ResponseWriter.Tests
{
    public class ResponseWriterTests
    {
        [Theory]
        [AutoData]
        public async Task ResponseWriter_ShowException_Default_ShouldNotShowExceptionProperty(
            HealthReport healthReport)
        {
            // Arrange
            var httpContext = CreateHttpContext();

            // Act
            await RockLibHealthChecks.ResponseWriter(httpContext, healthReport);

            // Assert
            var jsonBody = await GetJsonBody(httpContext);
            var healthCheckEntries = GetHealthCheckEntries(jsonBody);

            healthCheckEntries
                .Any(x => x["exception"] != null)
                .Should()
                .BeFalse();
        }

        [Theory]
        [AutoData]
        public async Task ResponseWriter_ShowException_SetToFalse_ShouldNotShowExceptionProperty(
            HealthReport healthReport)
        {
            // Arrange
            var httpContext = CreateHttpContext();
            RockLibHealthChecks.ShowExceptions = false;

            // Act
            await RockLibHealthChecks.ResponseWriter(httpContext, healthReport);

            // Assert
            var jsonBody = await GetJsonBody(httpContext);
            var healthCheckEntries = GetHealthCheckEntries(jsonBody);

            healthCheckEntries
                .Any(x => x["exception"] != null)
                .Should()
                .BeFalse();
        }

        [Theory]
        [AutoData]
        public async Task ResponseWriter_ShowException_SetToTrue_ShouldShowExceptionProperty(
            HealthReport healthReport)
        {
            // Arrange
            var httpContext = CreateHttpContext();
            RockLibHealthChecks.ShowExceptions = true;

            // Act
            await RockLibHealthChecks.ResponseWriter(httpContext, healthReport);

            // Assert
            var jsonBody = await GetJsonBody(httpContext);
            var healthCheckEntries = GetHealthCheckEntries(jsonBody);

            healthCheckEntries
                .Any(x => x["exception"] != null)
                .Should()
                .BeTrue();
        }

        [Theory]
        [AutoData]
        public async Task ResponseWriter_HideOutputs_Default_ShouldShowOutputProperty(
            HealthReport healthReport)
        {
            // Arrange
            var httpContext = CreateHttpContext();

            // Act
            await RockLibHealthChecks.ResponseWriter(httpContext, healthReport);

            // Assert
            var jsonBody = await GetJsonBody(httpContext);
            var healthCheckEntries = GetHealthCheckEntries(jsonBody);

            healthCheckEntries
                .Any(x => x["output"] != null)
                .Should()
                .BeTrue();
        }

        [Theory]
        [AutoData]
        public async Task ResponseWriter_HideOutputs_SetToFalse_ShouldShowOutputProperty(
            HealthReport healthReport)
        {
            // Arrange
            var httpContext = CreateHttpContext();
            RockLibHealthChecks.HideOutputs = false;

            // Act
            await RockLibHealthChecks.ResponseWriter(httpContext, healthReport);

            // Assert
            var jsonBody = await GetJsonBody(httpContext);
            var healthCheckEntries = GetHealthCheckEntries(jsonBody);

            healthCheckEntries
                .Any(x => x["output"] != null)
                .Should()
                .BeTrue();
        }

        [Theory]
        [AutoData]
        public async Task ResponseWriter_HideOutputs_SetToTrue_ShouldHideOutputProperty(
            HealthReport healthReport)
        {
            // Arrange
            var httpContext = CreateHttpContext();
            RockLibHealthChecks.HideOutputs = true;

            // Act
            await RockLibHealthChecks.ResponseWriter(httpContext, healthReport);

            // Assert
            var jsonBody = await GetJsonBody(httpContext);
            var healthCheckEntries = GetHealthCheckEntries(jsonBody);

            healthCheckEntries
                .Any(x => x["output"] != null)
                .Should()
                .BeFalse();
        }

        private static IEnumerable<JToken> GetHealthCheckEntries(string jsonBody) => JObject.Parse(jsonBody)
            .SelectToken("checks")
            .Children()
            .SelectMany(x => x.First);

        private static async Task<string> GetJsonBody(DefaultHttpContext httpContext)
        {
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            string bodyStr;
            using (var reader = new StreamReader(httpContext.Response.Body))
            {
                bodyStr = await reader.ReadToEndAsync();
            }

            return bodyStr;
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            return httpContext;
        }
    }
}