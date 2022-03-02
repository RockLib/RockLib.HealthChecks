using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace RockLib.HealthChecks.Client
{
    /// <summary>
    /// Represents the results of checking the health of a logical downstream dependency or sub-component.
    /// </summary>
    [JsonConverter(typeof(HealthCheckResultJsonConverter))]
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public sealed class HealthCheckResult : IDictionary<string, object?>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        private readonly IDictionary<string, object?> _dictionary
            = new ConcurrentDictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets a unique identifier of an instance of a specific sub-component/dependency of a
        /// service. Multiple objects with the same componentID MAY appear in the details, if they are from
        /// different nodes.
        /// </summary>
        [JsonIgnore]
        public string? ComponentId
        {
            get => TryGetValue("componentId", out string? value) ? value : null;
            set => _dictionary["componentId"] = value;
        }

        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        [JsonIgnore]
        public string? ComponentType
        {
            get => TryGetValue("componentType", out string? value) ? value : null;
            set => _dictionary["componentType"] = value;
        }

        /// <summary>
        /// Gets or sets the observed value.
        /// </summary>
        [JsonIgnore]
        public object? ObservedValue
        {
            get => TryGetValue("observedValue", out var value) ? value : null;
            set => _dictionary["observedValue"] = value;
        }

        /// <summary>
        /// Gets or sets the unit of measurement in which observedUnit is reported, e.g. for a time-based
        /// value it is important to know whether the time is reported in seconds, minutes, hours or
        /// something else.
        /// </summary>
        [JsonIgnore]
        public string? ObservedUnit
        {
            get => TryGetValue("observedUnit", out string? value) ? value : null;
            set => _dictionary["observedUnit"] = value;
        }

        /// <summary>
        /// Gets or sets whether the component status is acceptable or not.
        /// </summary>
        [JsonIgnore]
        public HealthStatus Status
        {
            get => TryGetValue("status", out HealthStatus status) ? status : default;
            set => _dictionary["status"] = value;
        }

        /// <summary>
        /// Gets or sets a list of URI Templates that indicate which endpoints are affected by the health
        /// check's troubles.
        /// </summary>
        [JsonIgnore]
#pragma warning disable CA1002 // Do not expose generic lists
        public List<string>? AffectedEndpoints
#pragma warning restore CA1002 // Do not expose generic lists
        {
            get => TryGetValue("affectedEndpoints", out List<string>? value) ? value : null;
            set => _dictionary["affectedEndpoints"] = value;
        }

        /// <summary>
        /// Gets or sets the date-time at which the reading of the <see cref="ObservedValue"/> was recorded.
        /// </summary>
        [JsonIgnore]
        public DateTime? Time
        {
            get => TryGetValue("time", out DateTime? date) ? date : null;
            set => _dictionary["time"] = value;
        }

        /// <summary>
        /// Gets or sets the raw error output, in case of <see cref="HealthStatus.Fail"/> or <see cref=
        /// "HealthStatus.Warn"/> states. This field SHOULD be omitted for <see cref="HealthStatus.Pass"/>
        /// state.
        /// </summary>
        [JsonIgnore]
        public string? Output
        {
            get => TryGetValue("output", out string? value) ? value : null;
            set => _dictionary["output"] = value;
        }

        /// <summary>
        /// Gets or sets a dictionary containing link relations and URIs [RFC3986] for external links that
        /// MAY contain more information about the health of the endpoint. All values of this object SHALL
        /// be URIs. Keys MAY also be URIs. Per web-linking standards [RFC8288] a link relationship SHOULD
        /// either be a common/registered one or be indicated as a URI, to avoid name clashes. If a 'self'
        /// link is provided, it MAY be used by clients to check health via HTTP response code.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string>? Links
        {
            get => TryGetValue("links", out Dictionary<string, string>? value) ? value : null;
            set => _dictionary["links"] = value;
        }

        private bool TryGetValue<T>(string key, [MaybeNullWhen(false)] out T value)
        {
            if (_dictionary.TryGetValue(key, out var obj))
            {
                if (obj is T t)
                {
                    value = t;
                    return true;
                }
                if (typeof(T).IsEnum)
                {
                    if(obj is T enumValue && Enum.IsDefined(typeof(T), enumValue))
                    {
                        value = enumValue;
                        return true;
                    }
                    else if(obj is string stringValue)
                    {
#if NET48
                        // There's no clean way to do this other than catching + swallowing an exception.
                        try
                        {
                            value = (T)Enum.Parse(typeof(T), obj.ToString(), true);
                            return true;
                        }
                        catch (Exception e) when (e is ArgumentNullException || e is ArgumentException || e is OverflowException) { }
#else
                        if (Enum.TryParse(typeof(T), stringValue, true, out var stringToEnumValue))
                        {
                            value = (T)stringToEnumValue!;
                            return true;
                        }
#endif
                    }
                }
            }

            value = default;
            return false;
        }

#region IDictionary<string, object> Members

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the keys of <see cref="HealthCheckResult"/>.
        /// </summary>
        public ICollection<string> Keys => _dictionary.Keys;

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the values in the <see cref="HealthCheckResult"/>.
        /// </summary>
        public ICollection<object?> Values => _dictionary.Values;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="HealthCheckResult"/>.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public object? this[string key] { get => _dictionary[key]; set => _dictionary[key] = value; }

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
        public void Add(string key, object? value) => _dictionary.Add(key, value);

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
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _dictionary.TryGetValue(key, out value);

        void ICollection<KeyValuePair<string, object?>>.Clear() => _dictionary.Clear();

        bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => _dictionary.IsReadOnly;

        void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item) => _dictionary.Add(item);

        bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item) => _dictionary.Contains(item);

        void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item) => _dictionary.Remove(item);

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

#endregion
    }
}
