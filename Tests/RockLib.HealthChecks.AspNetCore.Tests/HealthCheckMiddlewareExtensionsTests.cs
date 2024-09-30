using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using RockLib.HealthChecks.AspNetCore.Collector;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.HealthChecks.AspNetCore.Tests;

public static class HealthCheckMiddlewareExtensionsTests
{
    [Fact]
    public static void UseRockLibHealthChecksWithNullBuilder() =>
        Assert.Throws<ArgumentNullException>(() => (null as IApplicationBuilder)!.UseRockLibHealthChecks());

    [Fact]
    public static void UseRockLibHealthChecksWithNullName() =>
        Assert.Throws<ArgumentNullException>(() => Mock.Of<IApplicationBuilder>().UseRockLibHealthChecks(healthCheckRunnerName: null!));

    [Fact]
    public static void UseRockLibHealthChecksWithNullRoute() =>
        Assert.Throws<ArgumentNullException>(() => Mock.Of<IApplicationBuilder>().UseRockLibHealthChecks(route: null!));
    
    [Fact]
    public static void ConfigureRockLibHealthChecksRequiresBuilder() =>
        Assert.Throws<ArgumentNullException>(() => (null as IHostApplicationBuilder)!.ConfigureRockLibHealthChecks());

    [Fact]
    public static void ConfigureRockLibHealthChecksWithNoHealthChecks()
    {
        // no config here - not even a RockLib.HealthChecks section
        IConfiguration cfg = new ConfigurationBuilder().Build();
        
        var builder = new Mock<IHostApplicationBuilder>();
        builder.Setup(b => b.Configuration.GetSection("RockLib.HealthChecks"))
            .Returns(cfg.GetSection("RockLib.HealthChecks")); // returns an empty section
        
        builder.Object.ConfigureRockLibHealthChecks();
        
        // verify the config was attempted (implying the loader did not crash)
        builder.Verify(b => b.Configuration.GetSection("RockLib.HealthChecks"), Times.Once);
    }

    [Fact]
    public static void ConfigureRockLibHealthChecksWithMultipleHealthChecks()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "RockLib.HealthChecks:healthChecks", "[]" },
            { "RockLib.HealthChecks:healthChecks:0:type", "thisIsNotARealType" }, // should be overlooked gracefully
            { "RockLib.HealthChecks:healthChecks:1:type", "RockLib.HealthChecks.AspNetCore.Checks.MetricsHealthCheck, RockLib.HealthChecks.AspNetCore" }
        };
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        
        var builder = new Mock<IHostApplicationBuilder>();
        builder.Setup(b => b.Configuration.GetSection("RockLib.HealthChecks"))
            .Returns(config.GetSection("RockLib.HealthChecks"));
        
        // configure the builder with services
        var svcCollection = new ServiceCollection();
        builder.Setup(b => b.Services).Returns(svcCollection);
        
        // act
        builder.Object.ConfigureRockLibHealthChecks();
        
        // assert the configuration was read and the services were added
        builder.Verify(b => b.Configuration.GetSection("RockLib.HealthChecks"), Times.Once);
        Assert.Contains(svcCollection, sd => sd.ServiceType == typeof(IHealthMetricCollectorFactory));
    }
}
