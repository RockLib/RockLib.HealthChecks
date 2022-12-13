namespace RockLib.HealthChecks.AspNetCore;

/// <summary>
/// Defines an object that formats <see cref="HealthResponse"/> objects.
/// </summary>
public interface IResponseFormatter
{
    /// <summary>
    /// Formats the specified <see cref="HealthResponse"/>.
    /// </summary>
    /// <param name="healthResponse">The <see cref="HealthResponse"/> to format.</param>
    /// <returns>The formatted <see cref="HealthResponse"/>.</returns>
    string Format(HealthResponse healthResponse);
}
