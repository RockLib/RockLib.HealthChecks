using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RockLib.HealthChecks.Client
{
    /// <summary>
    /// Represents the response from a service to a health request.
    /// </summary>
    public class HealthResponse
    {
        /// <summary>
        /// Gets or sets whether the service status is acceptable or not.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("status")]
        public HealthStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the public version of the service.
        /// </summary>
        [JsonPropertyName("version")]
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the "release version" or "release ID" of the service.
        /// </summary>
        [JsonPropertyName("releaseId")]
        public string? ReleaseId { get; set; }

        /// <summary>
        /// Gets or sets a list of notes relevant to current state of health.
        /// </summary>
        [JsonPropertyName("notes")]
#pragma warning disable CA1002 // Do not expose generic lists
        public List<string>? Notes { get; set; }
#pragma warning restore CA1002 // Do not expose generic lists

        /// <summary>
        /// Gets or sets the raw error output, in case of <see cref="HealthStatus.Fail"/> or <see cref=
        /// "HealthStatus.Warn"/> states. This field SHOULD be omitted for <see cref="HealthStatus.Pass"/>
        /// state.
        /// </summary>
        [JsonPropertyName("output")]
        public string? Output { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier of the service, in the application scope.
        /// </summary>
        [JsonPropertyName("serviceId")]
        public string? ServiceId { get; set; }

        /// <summary>
        /// Gets or sets a human-friendly description of the service.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets the health check results of the logical downstream dependencies and sub-components of the
        /// service according the component name and measurement name of the health check result.
        /// </summary>
        [JsonPropertyName("checks")]
        public Dictionary<string, List<HealthCheckResult>>? Checks { get; set; }

        /// <summary>
        /// Gets or sets a dictionary containing link relations and URIs [RFC3986] for external links that
        /// MAY contain more information about the health of the endpoint. All values of this object SHALL
        /// be URIs. Keys MAY also be URIs. Per web-linking standards [RFC8288] a link relationship SHOULD
        /// either be a common/registered one or be indicated as a URI, to avoid name clashes. If a 'self'
        /// link is provided, it MAY be used by clients to check health via HTTP response code.
        /// </summary>
        [JsonPropertyName("links")]
        public Dictionary<string, string>? Links { get; set; }
    }
}
