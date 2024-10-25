using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.HealthChecks.Tests
{
    public class HealthCheckRunnerTests
    {
        [Fact]
        public void Constructor1SetsProperties()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };
            var name = "test-name";
            var description = "fake";
            var serviceId = "2.4.6";
            var version = "1.2.3";
            var releaseId = "4.5.6";
            var responseCustomizer = new Mock<IHealthResponseCustomizer>().Object;
            var contentType = "application/fake+json";
            var passStatusCode = 299;
            var warnStatusCode = 399;
            var failStatusCode = 599;

            var runner = new HealthCheckRunner(healthChecks, name, description, serviceId, version, releaseId,
                responseCustomizer, contentType, passStatusCode, warnStatusCode, failStatusCode);

            runner.HealthChecks.Should().BeEquivalentTo(healthChecks);
            runner.Name.Should().Be(name);
            runner.Description.Should().Be(description);
            runner.ServiceId.Should().Be(serviceId);
            runner.Version.Should().Be(version);
            runner.ReleaseId.Should().Be(releaseId);
            runner.ResponseCustomizer.Should().BeSameAs(responseCustomizer);
            runner.ContentType.Should().Be(contentType);
            runner.PassStatusCode.Should().Be(passStatusCode);
            runner.WarnStatusCode.Should().Be(warnStatusCode);
            runner.FailStatusCode.Should().Be(failStatusCode);
        }

        [Fact]
        public void Constructor2SetsProperties()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };
            var name = "test-name";
            var description = "fake";
            var serviceId = "2.4.6";
            var version = "1.2.3";
            var releaseId = "4.5.6";
            var responseCustomizer = new Mock<IHealthResponseCustomizer>().Object;
            var contentType = "application/fake+json";
            var passStatusCode = 299;
            var warnStatusCode = 399;
            var failStatusCode = 599;
            var uncaughtExceptionStatus = HealthStatus.Fail;

            var runner = new HealthCheckRunner(healthChecks, name, description, serviceId, version, releaseId,
                responseCustomizer, contentType, passStatusCode, warnStatusCode, failStatusCode, uncaughtExceptionStatus);

            runner.HealthChecks.Should().BeEquivalentTo(healthChecks);
            runner.Name.Should().Be(name);
            runner.Description.Should().Be(description);
            runner.ServiceId.Should().Be(serviceId);
            runner.Version.Should().Be(version);
            runner.ReleaseId.Should().Be(releaseId);
            runner.ResponseCustomizer.Should().BeSameAs(responseCustomizer);
            runner.ContentType.Should().Be(contentType);
            runner.PassStatusCode.Should().Be(passStatusCode);
            runner.WarnStatusCode.Should().Be(warnStatusCode);
            runner.FailStatusCode.Should().Be(failStatusCode);
            runner.UncaughtExceptionStatus.Should().Be(uncaughtExceptionStatus);
        }

        [Fact]
        public void ConstructorGivenNullContentTypeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, contentType: null!);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*contentType*");
        }

        [Fact]
        public void ConstructorGivenEmptyContentTypeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, contentType: "");

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*contentType*");
        }

        [Fact]
        public void ConstructorGivenTooLowPassStatusCodeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, passStatusCode: 199);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*passStatusCode*");
        }

        [Fact]
        public void ConstructorGivenTooHighPassStatusCodeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, passStatusCode: 400);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*passStatusCode*");
        }

        [Fact]
        public void ConstructorGivenTooLowWarnStatusCodeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, warnStatusCode: 199);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*warnStatusCode*");
        }

        [Fact]
        public void ConstructorGivenTooHighWarnStatusCodeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, warnStatusCode: 400);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*warnStatusCode*");
        }

        [Fact]
        public void ConstructorGivenTooLowFailStatusCodeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, failStatusCode: 399);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*failStatusCode*");
        }

        [Fact]
        public void ConstructorGivenTooHighFailStatusCodeThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, failStatusCode: 600);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*failStatusCode*");
        }

        [Fact]
        public void ConstructorGivenUndefinedUncaughtExceptionStatusThrows()
        {
            var healthChecks = new[] { new Mock<IHealthCheck>().Object, new Mock<IHealthCheck>().Object };

            Func<HealthCheckRunner> act = () => new HealthCheckRunner(healthChecks, uncaughtExceptionStatus: (HealthStatus)(-1));

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*uncaughtExceptionStatus*");
        }

        [Fact]
        public async Task RunAsyncRunsAllTheHealthChecksAndCallsTheHealthResponseCustomizer()
        {
            var mockHealthCheck1 = new Mock<IHealthCheck>();
            var result1 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results1 = new[] { result1 };
            mockHealthCheck1.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(results1));

            var mockHealthCheck2 = new Mock<IHealthCheck>();
            var result2 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results2 = new[] { result2 };
            mockHealthCheck2.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(results2));

            var mockResponseCustomizer = new Mock<IHealthResponseCustomizer>();

            var healthChecks = new[] { mockHealthCheck1.Object, mockHealthCheck2.Object };
            var responseCustomizer = mockResponseCustomizer.Object;

            var runner = new HealthCheckRunner(healthChecks, responseCustomizer: responseCustomizer);

            var response = await runner.RunAsync().ConfigureAwait(true);

            response.GetChecks().Should().BeEquivalentTo(new[] { result1, result2 });

            mockResponseCustomizer.Verify(m => m.CustomizeResponse(It.Is<HealthResponse>(responseParam => ReferenceEquals(response, responseParam))), Times.Once);

            mockHealthCheck1.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockHealthCheck2.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RunAsyncGivenAHealthCheckThatThrowsDoesNotThrow()
        {
            var mockHealthCheck1 = new Mock<IHealthCheck>();
            var result1 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results1 = new[] { result1 };
            mockHealthCheck1.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(results1));

            var mockHealthCheck2 = new Mock<IHealthCheck>();
            var result2 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results2 = new[] { result2 };
#pragma warning disable CA2201 // Do not raise reserved exception types
            var exception = new Exception();
#pragma warning restore CA2201 // Do not raise reserved exception types
            mockHealthCheck2.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Throws(exception);

            var healthChecks = new[] { mockHealthCheck1.Object, mockHealthCheck2.Object };
            var responseCustomizer = new Mock<IHealthResponseCustomizer>().Object;

            var runner = new HealthCheckRunner(healthChecks, responseCustomizer: responseCustomizer);

            var response = await runner.RunAsync().ConfigureAwait(true);

            response.GetChecks().Should().NotBeEquivalentTo(new[] { result1, result2 });

            mockHealthCheck1.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockHealthCheck2.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RunAsyncGivenNoResponseCustomizerDoesNotThrow()
        {
            var mockHealthCheck1 = new Mock<IHealthCheck>();
            var result1 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results1 = new[] { result1 };
            mockHealthCheck1.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(results1));

            var mockHealthCheck2 = new Mock<IHealthCheck>();
            var result2 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results2 = new[] { result2 };
            mockHealthCheck2.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(results2));

            var healthChecks = new[] { mockHealthCheck1.Object, mockHealthCheck2.Object };

            var runner = new HealthCheckRunner(healthChecks);

            var response = await runner.RunAsync().ConfigureAwait(true);

            response.GetChecks().Should().BeEquivalentTo(new[] { result1, result2 });

            mockHealthCheck1.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockHealthCheck2.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RunAsyncGivenAResponseCustomizerThatThrowsDoesNotThrow()
        {
            var mockHealthCheck1 = new Mock<IHealthCheck>();
            var result1 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results1 = new[] { result1 };
            mockHealthCheck1.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(results1));

            var mockHealthCheck2 = new Mock<IHealthCheck>();
            var result2 = new HealthCheckResult();
            IReadOnlyList<HealthCheckResult> results2 = new[] { result2 };
            mockHealthCheck2.Setup(m => m.CheckAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(results2));

            var mockResponseCustomizer = new Mock<IHealthResponseCustomizer>();
#pragma warning disable CA2201 // Do not raise reserved exception types
            var exception = new Exception();
#pragma warning restore CA2201 // Do not raise reserved exception types
            mockResponseCustomizer.Setup(m => m.CustomizeResponse(It.IsAny<HealthResponse>())).Throws(exception);

            var healthChecks = new[] { mockHealthCheck1.Object, mockHealthCheck2.Object };
            var responseCustomizer = mockResponseCustomizer.Object;

            var runner = new HealthCheckRunner(healthChecks, responseCustomizer: responseCustomizer);

            var response = await runner.RunAsync().ConfigureAwait(true);

            response.GetChecks().Should().BeEquivalentTo(new[] { result1, result2 });

            mockResponseCustomizer.Verify(m => m.CustomizeResponse(It.Is<HealthResponse>(responseParam => ReferenceEquals(response, responseParam))), Times.Once);

            mockHealthCheck1.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockHealthCheck2.Verify(m => m.CheckAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
