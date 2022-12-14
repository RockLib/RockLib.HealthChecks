using Moq;
using System.Web.Http;
using Xunit;

namespace RockLib.HealthChecks.WebApi.Tests;

public static class HealthCheckHandlerExtensionsTests
{
    [Fact]
    public static void MapHealthRoute()
    {
        using var routes = new HttpRouteCollection();
        routes.MapHealthRoute(Mock.Of<IHealthCheckRunner>());

        Assert.Multiple(
            () => Assert.Single(routes),
            () => Assert.IsType<HealthCheckHandler>(routes["HealthApi"].Handler)
            );
    }
}
