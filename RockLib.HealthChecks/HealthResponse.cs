using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.HealthChecks
{
    /// <summary>
    /// Represents the response from a service to a health request.
    /// </summary>
    public sealed class HealthResponse
    {
        private string _contentType = HealthCheckRunner.DefaultContentType;
        private int _statusCode = HealthCheckRunner.DefaultPassStatusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthResponse"/> class.
        /// </summary>
        /// <param name="results">
        /// The results of the health checks of the logical downstream dependencies and sub-components of
        /// the service.
        /// </param>
        public HealthResponse(IEnumerable<HealthCheckResult>? results = null)
        {
            if (results is not null)
            {
                Checks = results.ToLookup(x => x.GetKey(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);
                if (Checks.Count == 0)
                {
                    Checks = null;
                }
            }
            Status = GetChecks().Max(x => x.Status) ?? HealthStatus.Pass;
        }

        /// <summary>
        /// Gets or sets whether the service status is acceptable or not.
        /// </summary>
        [JsonProperty("status")]
        public HealthStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the public version of the service.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the "release version" or "release ID" of the service.
        /// </summary>
        [JsonProperty("releaseId", NullValueHandling = NullValueHandling.Ignore)]
        public string? ReleaseId { get; set; }

        /// <summary>
        /// Gets or sets a list of notes relevant to current state of health.
        /// </summary>
        [JsonProperty("notes", NullValueHandling = NullValueHandling.Ignore)]
#pragma warning disable CA1002 // Do not expose generic lists
        public List<string>? Notes { get; set; }
#pragma warning restore CA1002 // Do not expose generic lists

        /// <summary>
        /// Gets or sets the raw error output, in case of <see cref="HealthStatus.Fail"/> or <see cref=
        /// "HealthStatus.Warn"/> states. This field SHOULD be omitted for <see cref="HealthStatus.Pass"/>
        /// state.
        /// </summary>
        [JsonProperty("output", NullValueHandling = NullValueHandling.Ignore)]
        public string? Output { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier of the service, in the application scope.
        /// </summary>
        [JsonProperty("serviceId", NullValueHandling = NullValueHandling.Ignore)]
        public string? ServiceId { get; set; }

        /// <summary>
        /// Gets or sets a human-friendly description of the service.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets the health check results of the logical downstream dependencies and sub-components of the
        /// service according the component name and measurement name of the health check result.
        /// </summary>
        [JsonProperty("checks", NullValueHandling = NullValueHandling.Ignore)]
#pragma warning disable CA1721 // Property names should not match get methods
        public Dictionary<string, List<HealthCheckResult>>? Checks { get; }
#pragma warning restore CA1721 // Property names should not match get methods

        /// <summary>
        /// Gets or sets a dictionary containing link relations and URIs [RFC3986] for external links that
        /// MAY contain more information about the health of the endpoint. All values of this object SHALL
        /// be URIs. Keys MAY also be URIs. Per web-linking standards [RFC8288] a link relationship SHOULD
        /// either be a common/registered one or be indicated as a URI, to avoid name clashes. If a 'self'
        /// link is provided, it MAY be used by clients to check health via HTTP response code.
        /// </summary>
        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string>? Links { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code to be used with this health response. Value must be in the
        /// 200-599 range.
        /// </summary>
        [JsonIgnore]
        public int StatusCode
        {
            get => _statusCode;
            set
            {
                if (value < 200 || value > 599)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be in the 200-599 range.");
                }
                _statusCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the HTTP content type to be used with this health response.
        /// </summary>
        [JsonIgnore]
        public string ContentType
        {
            get => _contentType;
            set => _contentType = !string.IsNullOrEmpty(value) ? value : throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the health check results of the logical downstream dependencies and sub-components of the
        /// service.
        /// </summary>
        public IEnumerable<HealthCheckResult> GetChecks() =>
            Checks is not null ? Checks.Values.SelectMany(x => x) : Enumerable.Empty<HealthCheckResult>();

        /// <summary>
        /// Serializes this health response to JSON.
        /// </summary>
        /// <param name="indent">Whether to indent the resulting JSON.</param>
        /// <returns>A JSON representation of the health response.</returns>
        public string Serialize(bool indent = false) => JsonConvert.SerializeObject(this, indent ? Formatting.Indented : Formatting.None);
    }
}
