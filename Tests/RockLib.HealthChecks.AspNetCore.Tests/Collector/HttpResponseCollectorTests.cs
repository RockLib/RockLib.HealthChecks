using Moq;
using Moq.Protected;
using RockLib.HealthChecks.AspNetCore.Collector;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests.Collector;

public class HttpResponseCollectorTests
{
    [Fact]
    public async Task SendAsyncThrowsIfRequestIsNull()
    {
        using var handler = new HttpResponseCollector(new HealthMetricCollectorFactory());
        using var client = new HttpClient(handler);

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SendAsync(null!));
    }

    [Fact]
    public async Task SendAsyncCollectsOutcome()
    {
        var factory = new HealthMetricCollectorFactory();
        using var handler = new HttpResponseCollector(factory);
        using var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");

        var upstreamHandler = new Mock<DelegatingHandler>();
        handler.InnerHandler = upstreamHandler.Object;

        using var mockedResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

        upstreamHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockedResponse);

        //Act
        using var client = new HttpClient(handler);
        var actualResponse = await client.SendAsync(request);

        //check that internal call to SendAsync was only Once and with proper request object
        upstreamHandler.Protected()
            .Verify("SendAsync", Times.Once(), request, ItExpr.IsAny<CancellationToken>());

        Assert.Equal(HttpStatusCode.NoContent, actualResponse.StatusCode);
        var collector = factory.LeaseCollector("example.com");
        Assert.Single(collector.GetMetrics(x => x == (int)HttpStatusCode.NoContent));
    }
}