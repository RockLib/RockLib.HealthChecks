using RockLib.HealthChecks.HttpModule;
using System;

namespace WebForms.net45
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Set options for the health check http module in Application_Start.
            HealthCheckHttpModule.Route = "health";
            HealthCheckHttpModule.Indent = true;
        }
    }
}