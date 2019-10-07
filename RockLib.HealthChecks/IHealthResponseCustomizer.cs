namespace RockLib.HealthChecks
{
    /// <summary>
    /// Defines an object that customizes existing <see cref="HealthResponse"/> objects.
    /// </summary>
    public interface IHealthResponseCustomizer
    {
        /// <summary>
        /// Customize the values of the given <see cref="HealthResponse"/> object.
        /// </summary>
        /// <param name="response">The <see cref="HealthResponse"/> object to be customized.</param>
        /// <remarks>
        /// Consider modifying the values of the <see cref="HealthResponse.Status"/>, <see cref=
        /// "HealthResponse.StatusCode"/>, <see cref="HealthResponse.Notes"/>, <see cref=
        /// "HealthResponse.Output"/>, and <see cref="HealthResponse.Links"/> properties when implementing
        /// this method.
        /// </remarks>
        void CustomizeResponse(HealthResponse response);
    }
}