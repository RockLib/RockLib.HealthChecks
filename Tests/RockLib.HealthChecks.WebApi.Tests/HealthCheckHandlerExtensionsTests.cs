using Moq;
using System;
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

    [Fact]
    public static void MapHealthRouteWhenRoutesIsNull() => 
        Assert.Throws<ArgumentNullException>(
            () => (null as HttpRouteCollection)!.MapHealthRoute(Mock.Of<IHealthCheckRunner>()));

    [Fact]
    public static void MapHealthRouteWhenRouteIsNull()
    {
        using var routes = new HttpRouteCollection();
        Assert.Throws<ArgumentException>(
            () => routes.MapHealthRoute(Mock.Of<IHealthCheckRunner>(), route: null!));
    }

    [Fact]
    public static void MapHealthRouteWhenRouteIsEmpty()
    {
        using var routes = new HttpRouteCollection();
        Assert.Throws<ArgumentException>(
            () => routes.MapHealthRoute(Mock.Of<IHealthCheckRunner>(), route: string.Empty));
    }
}
