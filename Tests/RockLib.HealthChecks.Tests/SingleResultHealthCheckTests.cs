using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.Tests
{
    public class SingleResultHealthCheckTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            var healthCheck = new TestHealthCheck("FakeComponentName", "FakeMeasurementName", "FakeComponentType", "FakeComponentId");
            
            healthCheck.ComponentName.Should().Be("FakeComponentName");
            healthCheck.MeasurementName.Should().Be("FakeMeasurementName");
            healthCheck.ComponentType.Should().Be("FakeComponentType");
            healthCheck.ComponentId.Should().Be("FakeComponentId");
        }

        [Fact]
        public async Task CheckAsyncReturnsOneResult()
        {
            var healthCheck = new TestHealthCheck();

            var result = await healthCheck.CheckAsync().ConfigureAwait(false);

            result.Should().HaveCount(1);
            result[0]["fake"].Should().Be(true);
        }

        private sealed class TestHealthCheck : SingleResultHealthCheck
        {
            public TestHealthCheck(string? componentName = null, string? measurementName = null, string? componentType = null, string? componentId = null)
                : base(componentName, measurementName, componentType, componentId) { }
            
            protected override Task CheckAsync(HealthCheckResult result, CancellationToken cancellationToken)
            {
                result["fake"] = true;
                return Task.CompletedTask;
            }
        }
    }
}
