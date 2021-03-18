namespace RockLib.HealthChecks.Client
{
    /// <summary>
    /// Defines the values for the health status of a service or component.
    /// </summary>
    public enum HealthStatus
    {
        /// <summary>
        /// The service or component is healthy.
        /// </summary>
        Pass = 0,

        /// <summary>
        /// The service or component is healthy, with concerns.
        /// </summary>
        Warn = 1,

        /// <summary>
        /// The service or component is unhealthy.
        /// </summary>
        Fail = 2
    }
}
