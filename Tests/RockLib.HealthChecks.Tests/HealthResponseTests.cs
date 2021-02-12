using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RockLib.HealthChecks.Tests
{
    public class HealthResponseTests
    {
        [Fact]
        public void Constructor_SetsChecks()
        {
            var results = new[] { new HealthCheckResult(), new HealthCheckResult() };

            var response = new HealthResponse(results);

            response.Checks.Should().NotBeNull();
            response.GetChecks().Should().BeEquivalentTo(results);
        }

        [Fact]
        public void Constructor_GivenNullResults_DoesNotSetChecks()
        {
            IEnumerable<HealthCheckResult> results = null;

            var response = new HealthResponse(results);

            response.Checks.Should().BeNull();
            response.GetChecks().Should().BeEmpty();
        }

        [Fact]
        public void Constructor_GivenEmptyResults_DoesNotSetChecks()
        {
            var results = Enumerable.Empty<HealthCheckResult>();

            var response = new HealthResponse(results);

            response.Checks.Should().BeNull();
            response.GetChecks().Should().BeEmpty();
        }

        [Fact]
        public void Constructor_GivenNullResults_SetsStatusToPass()
        {
            IEnumerable<HealthCheckResult> results = null;

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Pass);
        }

        [Fact]
        public void Constructor_GivenEmptyResults_SetsStatusToPass()
        {
            var results = Enumerable.Empty<HealthCheckResult>();

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Pass);
        }

        [Fact]
        public void Constructor_GivenNonEmptyResultsWithNoStatus_SetsStatusToPass()
        {
            var results = new[] { new HealthCheckResult(), new HealthCheckResult() };

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Pass);
        }

        [Fact]
        public void Constructor_GivenNonEmptyResults_SetsStatusToHighestStatus1()
        {
            var results = new[]
            {
                new HealthCheckResult { Status = HealthStatus.Pass },
                new HealthCheckResult { Status = HealthStatus.Warn },
                new HealthCheckResult { Status = HealthStatus.Fail }
            };

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Fail);
        }

        [Fact]
        public void Constructor_GivenNonEmptyResults_SetsStatusToHighestStatus2()
        {
            var results = new[]
            {
                new HealthCheckResult { Status = HealthStatus.Pass },
                new HealthCheckResult { Status = HealthStatus.Warn },
                new HealthCheckResult { Status = HealthStatus.Warn }
            };

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Warn);
        }

        [Fact]
        public void Constructor_GivenNonEmptyResults_SetsStatusToHighestStatus3()
        {
            var results = new[]
            {
                new HealthCheckResult { Status = HealthStatus.Pass },
                new HealthCheckResult { Status = HealthStatus.Pass },
                new HealthCheckResult { Status = HealthStatus.Pass }
            };

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Pass);
        }
    }
}
