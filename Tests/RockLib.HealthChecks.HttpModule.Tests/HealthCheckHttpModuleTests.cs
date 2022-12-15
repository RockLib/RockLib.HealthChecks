using System;
using System.Web;
using Xunit;

namespace RockLib.HealthChecks.HttpModule.Tests;

public static class HealthCheckHttpModuleTests
{
    [Fact]
    public static void SetRoute()
    {
        HealthCheckHttpModule.Route = "/diagnostics";
        Assert.Equal("diagnostics", HealthCheckHttpModule.Route);
    }

    [Fact]
    public static void SetRouteWithNullValue() => 
        Assert.Throws<ArgumentException>(() => HealthCheckHttpModule.Route = null!);

    [Fact]
    public static void SetRouteWithEmptyValue() =>
        Assert.Throws<ArgumentException>(() => HealthCheckHttpModule.Route = string.Empty);

    [Fact]
    public static void Init()
    {
        var application = new HttpApplication();
        var module = new HealthCheckHttpModule();
        module.Init(application);
    }
}