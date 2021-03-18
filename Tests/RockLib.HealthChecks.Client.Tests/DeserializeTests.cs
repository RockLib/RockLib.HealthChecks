using FluentAssertions;
using System;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace RockLib.HealthChecks.Client.Tests
{
    public class DeserializeTests
    {
        [Fact(DisplayName = "Can deserialize a ResponseWriter output with Newtonsoft")]
        public void Test1()
        {
            const string response =
@"{
  ""status"": ""fail"",
  ""notes"": [
    ""TotalDuration: 00:00:00.0197315""
  ],
  ""checks"": {
      ""disk:space"": [{
          ""output"": null,
          ""exception"": null,
          ""duration"": ""00:00:00.0120517"",
          ""status"": ""pass""
      }],
      ""maxvalue"": [{
          ""output"": ""Maximum=10, Current=12"",
          ""exception"": ""Something went wrong"",
          ""duration"": ""00:00:00.0007031"",
          ""status"": ""fail""
      }]
  }
}";

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<HealthResponse>(response);

            result.Status.Should().Be(HealthStatus.Fail);
            result.Notes[0].Should().Be("TotalDuration: 00:00:00.0197315");

            result.Checks.Should().ContainKey("disk:space");
            result.Checks.Should().ContainKey("maxvalue");

            var checkResult1 = result.Checks["disk:space"].First();

            checkResult1.Output.Should().BeNull();
            checkResult1.Status.Should().Be(HealthStatus.Pass);
            checkResult1["duration"].Should().Be("00:00:00.0120517");
            checkResult1["exception"].Should().BeNull();

            var checkResult2 = result.Checks["maxvalue"].First();

            checkResult2.Output.Should().Be("Maximum=10, Current=12");
            checkResult2.Status.Should().Be(HealthStatus.Fail);
            checkResult2["duration"].Should().Be("00:00:00.0007031");
            checkResult2["exception"].Should().Be("Something went wrong");
        }

        [Fact(DisplayName = "Can deserialize a common RockLib HealthCheck output with Newtonsoft")]
        public void Test2()
        {
            const string response = @"{""status"":""pass"",""version"":""1"",""serviceId"":""3390b579-d076-4610-a9bd-7d1a5af893f9"",""description"":""My health check""
,""checks"":{""process: uptime"":[{""componentType"":""system"",""observedUnit"":""s"",""time"":""2021-03-11T18:15:47.7383888Z"",""status"":""pass"",""observedValue"":9.8543048999999989}]}}";

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<HealthResponse>(response);

            result.Status.Should().Be(HealthStatus.Pass);
            result.Version.Should().Be("1");
            result.ServiceId.Should().Be("3390b579-d076-4610-a9bd-7d1a5af893f9");
            result.Description.Should().Be("My health check");

            result.Checks.Should().ContainKey("process: uptime");

            var checkResult = result.Checks["process: uptime"].First();

            checkResult.ComponentType.Should().Be("system");
            checkResult.Time.Value.ToString("O").Should().Be("2021-03-11T18:15:47.7383888Z");
            checkResult.Status.Should().Be(HealthStatus.Pass);
            checkResult.ObservedValue.Should().Be(9.8543048999999989);
        }

        [Fact(DisplayName = "Can deserialize a ResponseWriter output with System.Text.Json")]
        public void Test3()
        {
            const string response =
@"{
  ""status"": ""fail"",
  ""notes"": [
    ""TotalDuration: 00:00:00.0197315""
  ],
  ""checks"": {
      ""disk:space"": [{
          ""output"": null,
          ""exception"": null,
          ""duration"": ""00:00:00.0120517"",
          ""status"": ""pass""
      }],
      ""maxvalue"": [{
          ""output"": ""Maximum=10, Current=12"",
          ""exception"": ""Something went wrong"",
          ""duration"": ""00:00:00.0007031"",
          ""status"": ""fail""
      }]
  }
}";

            var result = JsonSerializer.Deserialize<HealthResponse>(response);

            result.Status.Should().Be(HealthStatus.Fail);
            result.Notes[0].Should().Be("TotalDuration: 00:00:00.0197315");

            result.Checks.Should().ContainKey("disk:space");
            result.Checks.Should().ContainKey("maxvalue");

            var checkResult1 = result.Checks["disk:space"].First();

            checkResult1.Output.Should().BeNull();
            checkResult1.Status.Should().Be(HealthStatus.Pass);
            checkResult1["duration"].Should().Be("00:00:00.0120517");
            checkResult1["exception"].Should().BeNull();

            var checkResult2 = result.Checks["maxvalue"].First();

            checkResult2.Output.Should().Be("Maximum=10, Current=12");
            checkResult2.Status.Should().Be(HealthStatus.Fail);
            checkResult2["duration"].Should().Be("00:00:00.0007031");
            checkResult2["exception"].Should().Be("Something went wrong");
        }

        [Fact(DisplayName = "Can deserialize a common RockLib HealthCheck output with System.Text.Json")]
        public void Test4()
        {
            const string response = @"{""status"":""pass"",""version"":""1"",""serviceId"":""3390b579-d076-4610-a9bd-7d1a5af893f9"",""description"":""My health check""
,""checks"":{""process: uptime"":[{""componentType"":""system"",""observedUnit"":""s"",""time"":""2021-03-11T18:15:47.7383888Z"",""status"":""pass"",""observedValue"":9.8543048999999989}]}}";

            var result = JsonSerializer.Deserialize<HealthResponse>(response);

            result.Status.Should().Be(HealthStatus.Pass);
            result.Version.Should().Be("1");
            result.ServiceId.Should().Be("3390b579-d076-4610-a9bd-7d1a5af893f9");
            result.Description.Should().Be("My health check");

            result.Checks.Should().ContainKey("process: uptime");

            var checkResult = result.Checks["process: uptime"].First();

            checkResult.ComponentType.Should().Be("system");
            checkResult.Time.Value.ToString("O").Should().Be("2021-03-11T18:15:47.7383888Z");
            checkResult.Status.Should().Be(HealthStatus.Pass);
            checkResult.ObservedValue.Should().Be(9.8543048999999989M);
        }
    }
}
