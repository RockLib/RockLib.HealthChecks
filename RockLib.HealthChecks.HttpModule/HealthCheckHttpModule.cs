using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RockLib.HealthChecks.HttpModule
{
    /// <summary>
    /// HTTP Module for scanning and running Health Checks.
    /// </summary>
    public sealed class HealthCheckHttpModule : IHttpModule
    {
        private static string _route = "health";
        private Regex _healthCheckRouteRegex;

        /// <summary>
        /// Gets or sets the route of the health endpoint.
        /// </summary>
        public static string Route
        {
            get => _route;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));
                _route = value.Trim('/');
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to indent the JSON output.
        /// </summary>
        public static bool Indent { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="IHealthCheckRunner"/>.
        /// </summary>
        public static string HealthCheckRunnerName { get; set; }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Initializes this HTTP Module with the Health Check Settings found in your app.
        /// </summary>
        /// <param name="context">The application's context.</param>
        public void Init(HttpApplication context)
        {
            _healthCheckRouteRegex = new Regex($@"^\/*({Route})\/*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            try
            {
                context.AuthenticateRequest -= AuthenticateRequest;
                context.AuthenticateRequest += AuthenticateRequest;

                var healthCheckAsyncEventHelper = new EventHandlerTaskAsyncHelper(ExecuteHealthChecksEventAsync);
                context.AddOnPostAcquireRequestStateAsync(healthCheckAsyncEventHelper.BeginEventHandler, healthCheckAsyncEventHelper.EndEventHandler);
            }
            catch { }
        }

        private void AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                var context = GetHttpContext(sender);

                if (ShouldDoHealthCheck(context?.Request?.RawUrl))
                {
                    context.SkipAuthorization = true;
                }
            }
            catch { }
        }

        private async Task ExecuteHealthChecksEventAsync(object sender, EventArgs e)
        {
            try
            {
                var context = GetHttpContext(sender);

                if (ShouldDoHealthCheck(context?.Request?.RawUrl))
                {
                    var healthCheckResponse = await HealthCheck.GetRunner(HealthCheckRunnerName).RunAsync().ConfigureAwait(false);

                    context.Response.Clear();
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.StatusCode = healthCheckResponse.StatusCode;
                    context.Response.ContentType = healthCheckResponse.ContentType;
                    context.Response.Write(healthCheckResponse.Serialize(Indent));
                    context.Response.End();
                }
            }
            catch { }
        }

        private HttpContext GetHttpContext(object sender)
        {
            return (sender as HttpApplication)?.Context;
        }

        private bool ShouldDoHealthCheck(string url)
        {
            if (url == null)
            {
                return false;
            }

            return _healthCheckRouteRegex.IsMatch(url);
        }
    }
}
