using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RockLib.HealthChecks
{
    /// <summary>
    /// Defines the values for the health status of a service or component.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HealthStatus
    {
        /// <summary>
        /// The service or component is healthy.
        /// </summary>
        [EnumMember(Value = "pass")]
        Pass = 0,

        /// <summary>
        /// The service or component is healthy, with concerns.
        /// </summary>
        [EnumMember(Value = "warn")]
        Warn = 1,

        /// <summary>
        /// The service or component is unhealthy.
        /// </summary>
        [EnumMember(Value = "fail")]
        Fail = 2
    }
}
