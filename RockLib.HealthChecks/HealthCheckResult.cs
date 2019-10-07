using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
#if !(NET35 || NET40)
using System.Collections.Concurrent;
#endif

namespace RockLib.HealthChecks
{
    /// <summary>
    /// Represents the results of checking the health of a logical downstream dependency or sub-component.
    /// </summary>
    public sealed class HealthCheckResult : IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _dictionary
#if NET35 || NET40
            = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
#else
            = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
#endif

        private string _componentName;
        private string _measurementName;

        /// <summary>
        /// Gets or sets the human-readable name for the component. Must not contain a colon.
        /// </summary>
        [JsonIgnore]
        public string ComponentName
        {
            get => _componentName;
            set => _componentName = DisallowColon(value);
        }

        /// <summary>
        /// Gets or sets the name of the measurement type (a data point type) that the status is reported for.
        /// </summary>
        [JsonIgnore]
        public string MeasurementName
        {
            get => _measurementName;
            set => _measurementName = DisallowColon(value);
        }

        /// <summary>
        /// Gets or sets a unique identifier of an instance of a specific sub-component/dependency of a
        /// service. Multiple objects with the same componentID MAY appear in the details, if they are from
        /// different nodes.
        /// </summary>
        [JsonIgnore]
        public string ComponentId
        {
            get => TryGetValue("componentId", out string value) ? value : null;
            set => SetValue("componentId", value);
        }

        /// <summary>
        /// Gets or sets the type of the component. SHOULD be present if <see cref="ComponentName"/> is
        /// present.
        /// </summary>
        [JsonIgnore]
        public string ComponentType
        {
            get => TryGetValue("componentType", out string value) ? value : null;
            set => SetValue("componentType", value);
        }

        /// <summary>
        /// Gets or sets the observed value.
        /// </summary>
        [JsonIgnore]
        public object ObservedValue
        {
            get => TryGetValue("observedValue", out object value) ? value : null;
            set => SetValue("observedValue", value);
        }

        /// <summary>
        /// Gets or sets the unit of measurement in which observedUnit is reported, e.g. for a time-based
        /// value it is important to know whether the time is reported in seconds, minutes, hours or
        /// something else.
        /// </summary>
        [JsonIgnore]
        public string ObservedUnit
        {
            get => TryGetValue("observedUnit", out string value) ? value : null;
            set => SetValue("observedUnit", value);
        }

        /// <summary>
        /// Gets or sets whether the component status is acceptable or not.
        /// </summary>
        [JsonIgnore]
        public HealthStatus? Status
        {
            get => TryGetValue("status", out HealthStatus? value) ? value : null;
            set => SetValue("status", value);
        }

        /// <summary>
        /// Gets or sets a list of URI Templates that indicate which endpoints are affected by the health
        /// check's troubles.
        /// </summary>
        [JsonIgnore]
        public List<string> AffectedEndpoints
        {
            get => TryGetValue("affectedEndpoints", out List<string> value) ? value : null;
            set => SetValue("affectedEndpoints", value);
        }

        /// <summary>
        /// Gets or sets the date-time at which the reading of the <see cref="ObservedValue"/> was recorded.
        /// </summary>
        [JsonIgnore]
        public DateTime? Time
        {
            get => TryGetValue("time", out DateTime? value) ? value : null;
            set => SetValue("time", EnsureUtcTime(value));
        }

        /// <summary>
        /// Gets or sets the raw error output, in case of <see cref="HealthStatus.Fail"/> or <see cref=
        /// "HealthStatus.Warn"/> states. This field SHOULD be omitted for <see cref="HealthStatus.Pass"/>
        /// state.
        /// </summary>
        [JsonIgnore]
        public string Output
        {
            get => TryGetValue("output", out string value) ? value : null;
            set => SetValue("output", value);
        }

        /// <summary>
        /// Gets or sets a dictionary containing link relations and URIs [RFC3986] for external links that
        /// MAY contain more information about the health of the endpoint. All values of this object SHALL
        /// be URIs. Keys MAY also be URIs. Per web-linking standards [RFC8288] a link relationship SHOULD
        /// either be a common/registered one or be indicated as a URI, to avoid name clashes. If a 'self'
        /// link is provided, it MAY be used by clients to check health via HTTP response code.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string> Links
        {
            get => TryGetValue("links", out Dictionary<string, string> value) ? value : null;
            set => SetValue("links", value);
        }

        private bool TryGetValue<T>(string key, out T value)
        {
            if (_dictionary.TryGetValue(key, out object obj) && obj is T t)
            {
                value = t;
                return true;
            }

            value = default(T);
            return false;
        }

        private void SetValue(string key, object value)
        {
            if (value == null)
                _dictionary.Remove(key);
            else
                _dictionary[key] = value;
        }

        internal string GetKey()
        {
            var emptyComponentName = string.IsNullOrEmpty(ComponentName);
            var emptyMeasurementName = string.IsNullOrEmpty(MeasurementName);

            if (emptyComponentName && emptyMeasurementName)
                return "";
            if (emptyComponentName)
                return MeasurementName;
            if (emptyMeasurementName)
                return ComponentName;
            return $"{ComponentName}:{MeasurementName}";
        }

        private static string DisallowColon(string value)
        {
            if (value != null && value.Contains(':'))
                throw new ArgumentException("value cannot contain ':' (colon) character.", nameof(value));
            return value;
        }

        private static DateTime? EnsureUtcTime(DateTime? value)
        {
            return value.HasValue && value.Value.Kind != DateTimeKind.Utc
                ? value.Value.ToUniversalTime()
                : value;
        }

        #region IDictionary<string, object> Members

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the keys of <see cref="HealthCheckResult"/>.
        /// </summary>
        [JsonIgnore]
        public ICollection<string> Keys => _dictionary.Keys;

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the values in the <see cref="HealthCheckResult"/>.
        /// </summary>
        [JsonIgnore]
        public ICollection<object> Values => _dictionary.Values;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="HealthCheckResult"/>.
        /// </summary>
        [JsonIgnore]
        public int Count => _dictionary.Count;

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public object this[string key] { get => _dictionary[key]; set => SetValue(key, value); }

        /// <summary>
        /// Determines whether the <see cref="HealthCheckResult"/> contains an element with
        /// the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="HealthCheckResult"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="HealthCheckResult"/> contains an element
        /// with the key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="HealthCheckResult"/>.
        /// </summary>
        /// <param name="key">The string to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, object value) => SetValue(key, value);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="HealthCheckResult"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the element is successfully removed; otherwise,
        /// <see langword="false"/>. This method also returns <see langword="false"/>
        /// if key was not found in the original <see cref="HealthCheckResult"/>.
        /// </returns>
        public bool Remove(string key) => _dictionary.Remove(key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the
        /// key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="HealthCheckResult"/> contains an element
        /// with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(string key, out object value) => _dictionary.TryGetValue(key, out value);

        void ICollection<KeyValuePair<string, object>>.Clear() => _dictionary.Clear();

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => _dictionary.IsReadOnly;

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) => SetValue(item.Key, item.Value);

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) => _dictionary.Contains(item);

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
        
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => _dictionary.Remove(item);
        
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => _dictionary.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
        
#endregion
    }
}
