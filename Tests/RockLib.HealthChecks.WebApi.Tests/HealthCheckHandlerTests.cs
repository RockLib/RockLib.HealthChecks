using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.WebApi.Tests;

public static class HealthCheckHandlerTests
{
    [Fact]
    public static void CreateWithNullRunner() =>
        Assert.Throws<ArgumentNullException>(() => new HealthCheckHandler(null!, true));

    [Fact]
    public static async Task SendAsync()
    {
        var response = new HealthResponse() { StatusCode = (int)HttpStatusCode.OK, ContentType = "text/json" };
        var runner = new Mock<IHealthCheckRunner>(MockBehavior.Strict);
        runner.Setup(_ => _.RunAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));

        using var handler = new HealthCheckHandler(runner.Object, true);
        using var client = new HttpClient(handler) { BaseAddress = new("http://localhost") };

        var clientResponse = await client.GetAsync(new Uri("http://localhost/health")).ConfigureAwait(false);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode),
            async () => Assert.Equal("", await clientResponse.Content.ReadAsStringAsync().ConfigureAwait(false))
            );

        runner.VerifyAll();
    }
}
