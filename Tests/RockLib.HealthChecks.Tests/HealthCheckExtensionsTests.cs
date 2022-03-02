using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace RockLib.HealthChecks.Tests
{
    public class HealthCheckExtensionsTests
    {
        [Fact]
        public void CreateHealthCheckResultSetsPropertiesOfResult()
        {
            var mockHealthCheck = new Mock<IHealthCheck>();

            mockHealthCheck.Setup(m => m.ComponentName).Returns("FakeComponentName");
            mockHealthCheck.Setup(m => m.MeasurementName).Returns("FakeMeasurementName");
            mockHealthCheck.Setup(m => m.ComponentType).Returns("FakeComponentType");
            mockHealthCheck.Setup(m => m.ComponentId).Returns("FakeComponentId");

            var healthCheck = mockHealthCheck.Object;

            var beforeTime = DateTime.UtcNow;

            var result = healthCheck.CreateHealthCheckResult();

            var afterTime = DateTime.UtcNow;

            result.ComponentName.Should().Be("FakeComponentName");
            result.MeasurementName.Should().Be("FakeMeasurementName");
            result.ComponentType.Should().Be("FakeComponentType");
            result.ComponentId.Should().Be("FakeComponentId");
            result.Time.Should().BeOnOrAfter(beforeTime).And.BeOnOrBefore(afterTime);
        }

        [Fact]
        public void CreateHealthCheckResultGivenNullHealthCheckThrows()
        {
            IHealthCheck healthCheck = null!;

            Func<HealthCheckResult> act = () => healthCheck!.CreateHealthCheckResult();

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*healthCheck*");
        }

        [Fact]
        public void CreateHealthResponseSetsPropertiesOfResponse()
        {
            var mockRunner = new Mock<IHealthCheckRunner>();

            mockRunner.Setup(m => m.Version).Returns("FakeVersion");
            mockRunner.Setup(m => m.ReleaseId).Returns("FakeReleaseId");
            mockRunner.Setup(m => m.ServiceId).Returns("FakeServiceId");
            mockRunner.Setup(m => m.Description).Returns("FakeDescription");
            mockRunner.Setup(m => m.ContentType).Returns("FakeContentType");
            mockRunner.Setup(m => m.PassStatusCode).Returns(299);
            mockRunner.Setup(m => m.WarnStatusCode).Returns(399);
            mockRunner.Setup(m => m.FailStatusCode).Returns(599);

            var runner = mockRunner.Object;
            var results = new[] { new HealthCheckResult { Status = HealthStatus.Warn } };

            var response = runner.CreateHealthResponse(results);

            response.Version.Should().Be(runner.Version);
            response.ReleaseId.Should().Be(runner.ReleaseId);
            response.ServiceId.Should().Be(runner.ServiceId);
            response.Description.Should().Be(runner.Description);
            response.ContentType.Should().Be(runner.ContentType);
            response.Status.Should().Be(HealthStatus.Warn);
            response.StatusCode.Should().Be(399);
            response.GetChecks().Should().BeEquivalentTo(results);
        }

        [Theory]
        [InlineData(HealthStatus.Pass, 299)]
        [InlineData(HealthStatus.Warn, 399)]
        [InlineData(HealthStatus.Fail, 599)]
        public void SetStatusCodeSetsStatusCode(HealthStatus status, int expectedStatusCode)
        {
            var mockRunner = new Mock<IHealthCheckRunner>();

            mockRunner.Setup(m => m.PassStatusCode).Returns(299);
            mockRunner.Setup(m => m.WarnStatusCode).Returns(399);
            mockRunner.Setup(m => m.FailStatusCode).Returns(599);

            var runner = mockRunner.Object;
            var response = new HealthResponse { Status = status };
            
            response.SetStatusCode(runner);

            response.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
