using Microsoft.AspNetCore.Builder;
using Moq;
using System;
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
}
