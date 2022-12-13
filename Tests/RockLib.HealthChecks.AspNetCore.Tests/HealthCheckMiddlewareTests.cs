using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests;

public static class HealthCheckMiddlewareTests
{
    [Fact]
    public static void CreateWithNullRunner() =>
        Assert.Throws<ArgumentNullException>(() => new HealthCheckMiddleware(new RequestDelegate(context => Task.CompletedTask), null!));

    [Fact]
    public static async Task InvokeAsync()
    {
        var response = new HealthResponse() { StatusCode = 200, ContentType = "Custom" };
        var responseStream = new MemoryStream();

        var runner = new Mock<IHealthCheckRunner>(MockBehavior.Strict);
        runner.Setup(_ => _.RunAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));

        var statusCode = 0;
        var contentType = "";

        var contextResponse = new Mock<HttpResponse>(MockBehavior.Strict);
        contextResponse.SetupSet(_ => _.StatusCode = It.IsAny<int>()).Callback<int>(_ => statusCode = _);
        contextResponse.SetupSet(_ => _.ContentType = It.IsAny<string>()).Callback<string>(_ => contentType = _);
        contextResponse.SetupGet(_ => _.Body).Returns(responseStream);
        //contextResponse.Setup(_ => _.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var context = new Mock<HttpContext>(MockBehavior.Strict);
        context.SetupGet(_ => _.Response).Returns(contextResponse.Object);
        context.SetupGet(_ => _.RequestAborted).Returns(new CancellationToken());

        var middleware = new HealthCheckMiddleware(new RequestDelegate(context => Task.CompletedTask), runner.Object);
        await middleware.InvokeAsync(context.Object).ConfigureAwait(false);

        Assert.Equal(200, statusCode); 
        Assert.Equal("Custom", contentType);
        Assert.True(responseStream.Length > 0);

        context.VerifyAll();
        contextResponse.VerifyAll();
        runner.VerifyAll();
    }
}
