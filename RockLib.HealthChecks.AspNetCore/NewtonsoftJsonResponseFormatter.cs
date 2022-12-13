using Newtonsoft.Json;
using System;

namespace RockLib.HealthChecks.AspNetCore;

/// <summary>
/// An implementation of <see cref="IResponseFormatter"/> that formats health responses as draft
/// RFC compliant JSON using Newtonsoft.Json.
/// </summary>
public class NewtonsoftJsonResponseFormatter : IResponseFormatter
{
    internal static readonly NewtonsoftJsonResponseFormatter DefaultInstance = new NewtonsoftJsonResponseFormatter();

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonResponseFormatter"/> class.
    /// </summary>
    /// <param name="formatting">The formatting options.</param>
    /// <param name="settings">The JSON serializer settings.</param>
    public NewtonsoftJsonResponseFormatter(Formatting formatting = Formatting.None, JsonSerializerSettings? settings = null)
    {
        if (!Enum.IsDefined(typeof(Formatting), formatting))
        {
            throw new ArgumentOutOfRangeException(nameof(formatting));
        }

        Formatting = formatting;
        Settings = settings;
    }

    /// <summary>
    /// Gets the formatting options.
    /// </summary>
    public Formatting Formatting { get; }

    /// <summary>
    /// Gets the JSON serializer settings.
    /// </summary>
    public JsonSerializerSettings? Settings { get; }

    /// <summary>
    /// Formats the specified <see cref="HealthResponse"/> as draft RFC compliant JSON.
    /// </summary>
    /// <param name="healthResponse">The <see cref="HealthResponse"/> to format.</param>
    /// <returns>The formatted <see cref="HealthResponse"/>.</returns>
    public string Format(HealthResponse healthResponse)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(healthResponse);
#else
        if (healthResponse is null) { throw new ArgumentNullException(nameof(healthResponse)); }
#endif

        return JsonConvert.SerializeObject(healthResponse, typeof(HealthResponse), Formatting, Settings);
    }
}
