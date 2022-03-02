using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RockLib.HealthChecks.Tests
{
    public class HealthResponseTests
    {
        [Fact]
        public void ConstructorSetsChecks()
        {
            var results = new[] { new HealthCheckResult(), new HealthCheckResult() };

            var response = new HealthResponse(results);

            response.Checks.Should().NotBeNull();
            response.GetChecks().Should().BeEquivalentTo(results);
        }

        [Fact]
        public void ConstructorGivenNullResultsDoesNotSetChecks()
        {
            IEnumerable<HealthCheckResult> results = null!;

            var response = new HealthResponse(results);

            response.Checks.Should().BeNull();
            response.GetChecks().Should().BeEmpty();
        }

        [Fact]
        public void ConstructorGivenEmptyResultsDoesNotSetChecks()
        {
            var results = Enumerable.Empty<HealthCheckResult>();

            var response = new HealthResponse(results);

            response.Checks.Should().BeNull();
            response.GetChecks().Should().BeEmpty();
        }

        [Fact]
        public void ConstructorGivenNullResultsSetsStatusToPass()
        {
            IEnumerable<HealthCheckResult> results = null!;

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Pass);
        }

        [Fact]
        public void ConstructorGivenEmptyResultsSetsStatusToPass()
        {
            var results = Enumerable.Empty<HealthCheckResult>();

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Pass);
        }

        [Fact]
        public void ConstructorGivenNonEmptyResultsWithNoStatusSetsStatusToPass()
        {
            var results = new[] { new HealthCheckResult(), new HealthCheckResult() };

            var response = new HealthResponse(results);

            response.Status.Should().Be(HealthStatus.Pass);
        }

        [Fact]
        public void ConstructorGivenNonEmptyResultsSetsStatusToHighestStatus1()
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
        public void ConstructorGivenNonEmptyResultsSetsStatusToHighestStatus2()
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
        public void ConstructorGivenNonEmptyResultsSetsStatusToHighestStatus3()
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
