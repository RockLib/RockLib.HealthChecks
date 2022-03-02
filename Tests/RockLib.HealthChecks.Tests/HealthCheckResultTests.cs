using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.HealthChecks.Tests
{
    public class HealthCheckResultTests
    {
        [Fact]
        public void SetComponentNameDoesNotChangeDictionary()
        {
            var result = new HealthCheckResult();

            result.ComponentName = "foo";

            result.Should().BeEmpty();
        }

        [Fact]
        public void SetComponentNameGivenValueWithColonThrows()
        {
            var result = new HealthCheckResult();

            Action act = () => result.ComponentName = "foo:bar";

            act.Should().ThrowExactly<ArgumentException>().WithMessage("*colon*");
        }

        [Fact]
        public void SetMeasurementNameDoesNotChangeDictionary()
        {
            var result = new HealthCheckResult();

            result.MeasurementName = "bar";

            result.Should().BeEmpty();
        }

        [Fact]
        public void SetMeasurementNameGivenValueWithColonThrows()
        {
            var result = new HealthCheckResult();

            Action act = () => result.MeasurementName = "foo:bar";

            act.Should().ThrowExactly<ArgumentException>().WithMessage("*colon*");
        }

        [Fact]
        public void GetKeyGivenNoComponentNameOrMeasureNameReturnsEmptyString()
        {
            var result = new HealthCheckResult();

            var key = result.GetKey();

            key.Should().BeEmpty();
        }

        [Fact]
        public void GetKeyGivenNoMeasurementNameReturnsComponentName()
        {
            var result = new HealthCheckResult { ComponentName = "foo" };

            var key = result.GetKey();

            key.Should().Be("foo");
        }

        [Fact]
        public void GetKeyGivenNoComponentNameReturnsMeasurementName()
        {
            var result = new HealthCheckResult { MeasurementName = "foo" };

            var key = result.GetKey();

            key.Should().Be("foo");
        }

        [Fact]
        public void GetKeyGivenComponentNameAndMeasurementNameReturnsBoth()
        {
            var result = new HealthCheckResult { ComponentName = "foo", MeasurementName = "bar" };

            var key = result.GetKey();

            key.Should().Be("foo:bar");
        }

        [Fact]
        public void PropertyGettersReturnDictionaryValues()
        {
            var now = DateTime.UtcNow;

            var result = new HealthCheckResult
            {
                ["componentId"] = "FakeComponentId",
                ["componentType"] = "FakeComponentType",
                ["observedValue"] = "FakeObservedValue",
                ["observedUnit"] = "FakeObservedUnit",
                ["status"] = HealthStatus.Warn,
                ["affectedEndpoints"] = new List<string> { "FakeAffectedEndpoint" },
                ["time"] = now,
                ["output"] = "FakeOutput",
                ["links"] = new Dictionary<string, string> { { "foo", "bar" }, { "baz", "qux" } }
            };

            result.ComponentId.Should().Be("FakeComponentId");
            result.ComponentType.Should().Be("FakeComponentType");
            result.ObservedValue.Should().Be("FakeObservedValue");
            result.ObservedUnit.Should().Be("FakeObservedUnit");
            result.Status.Should().Be(HealthStatus.Warn);
            result.AffectedEndpoints.Should().BeEquivalentTo(new[] { "FakeAffectedEndpoint" });
            result.Time.Should().Be(now);
            result.Output.Should().Be("FakeOutput");
            result.Links.Should().BeEquivalentTo(new Dictionary<string, string> { { "foo", "bar" }, { "baz", "qux" } });
        }

        [Fact]
        public void PropertyGettersWhenFoundValueTypeDoesNotMatchExpectedTypeReturnNull()
        {
            var result = new HealthCheckResult
            {
                ["componentId"] = 123,
                ["componentType"] = 123,
                ["observedUnit"] = 123,
                ["status"] = DateTime.UtcNow,
                ["affectedEndpoints"] = 123,
                ["time"] = 123,
                ["output"] = 123,
                ["links"] = 123
            };

            result.ComponentId.Should().BeNull();
            result.ComponentType.Should().BeNull();
            result.ObservedUnit.Should().BeNull();
            result.Status.Should().BeNull();
            result.AffectedEndpoints.Should().BeNull();
            result.Time.Should().BeNull();
            result.Output.Should().BeNull();
            result.Links.Should().BeNull();
        }

        [Fact]
        public void PropertySettersSetDictionaryValues()
        {
            var now = DateTime.UtcNow;

            var result = new HealthCheckResult
            {
                ComponentId = "FakeComponentId",
                ComponentType = "FakeComponentType",
                ObservedValue = "FakeObservedValue",
                ObservedUnit = "FakeObservedUnit",
                Status = HealthStatus.Warn,
                AffectedEndpoints = new List<string> { "FakeAffectedEndpoint" },
                Time = now,
                Output = "FakeOutput",
                Links = new Dictionary<string, string> { { "foo", "bar" }, { "baz", "qux" } }
            };

            result["componentId"].Should().Be("FakeComponentId");
            result["componentType"].Should().Be("FakeComponentType");
            result["observedValue"].Should().Be("FakeObservedValue");
            result["observedUnit"].Should().Be("FakeObservedUnit");
            result["status"].Should().Be(HealthStatus.Warn);
            result["affectedEndpoints"].Should().BeEquivalentTo(new[] { "FakeAffectedEndpoint" });
            result["time"].Should().Be(now);
            result["output"].Should().Be("FakeOutput");
            result["links"].Should().BeEquivalentTo(new Dictionary<string, string> { { "foo", "bar" }, { "baz", "qux" } });
        }

        [Fact]
        public void PropertySettersWhenPassedNullSetsNullValue()
        {
            var now = DateTime.UtcNow;

            var result = new HealthCheckResult
            {
                ComponentId = "FakeComponentId",
                ComponentType = "FakeComponentType",
                ObservedValue = "FakeObservedValue",
                ObservedUnit = "FakeObservedUnit",
                Status = HealthStatus.Warn,
                AffectedEndpoints = new List<string> { "FakeAffectedEndpoint" },
                Time = now,
                Output = "FakeOutput",
                Links = new Dictionary<string, string> { { "foo", "bar" }, { "baz", "qux" } }
            };

            result.ComponentId = null;
            result.ComponentType = null;
            result.ObservedValue = null;
            result.ObservedUnit = null;
            result.Status = null;
            result.AffectedEndpoints = null;
            result.Time = null;
            result.Output = null;
            result.Links = null;

            result["componentId"].Should().BeNull();
            result["componentType"].Should().BeNull();
            result["observedValue"].Should().BeNull();
            result["observedUnit"].Should().BeNull();
            result["status"].Should().BeNull();
            result["affectedEndpoints"].Should().BeNull();
            result["time"].Should().BeNull();
            result["output"].Should().BeNull();
            result["links"].Should().BeNull();
        }

        [Fact]
        public void SettingDictionaryValuesToNullSetsPropertyValuesToNull()
        {
            var now = DateTime.UtcNow;

            var result = new HealthCheckResult
            {
                ["componentId"] = "FakeComponentId",
                ["componentType"] = "FakeComponentType",
                ["observedValue"] = "FakeObservedValue",
                ["observedUnit"] = "FakeObservedUnit",
                ["status"] = HealthStatus.Warn,
                ["affectedEndpoints"] = new List<string> { "FakeAffectedEndpoint" },
                ["time"] = now,
                ["output"] = "FakeOutput",
                ["links"] = new Dictionary<string, string> { { "foo", "bar" }, { "baz", "qux" } }
            };

            result["componentId"] = null;
            result["componentType"] = null;
            result["observedValue"] = null;
            result["observedUnit"] = null;
            result["status"] = null;
            result["affectedEndpoints"] = null;
            result["time"] = null;
            result["output"] = null;
            result["links"] = null;

            result.ComponentId.Should().BeNull();
            result.ComponentType.Should().BeNull();
            result.ObservedValue.Should().BeNull();
            result.ObservedUnit.Should().BeNull();
            result.Status.Should().BeNull();
            result.AffectedEndpoints.Should().BeNull();
            result.Time.Should().BeNull();
            result.Output.Should().BeNull();
            result.Links.Should().BeNull();
        }
    }
}
