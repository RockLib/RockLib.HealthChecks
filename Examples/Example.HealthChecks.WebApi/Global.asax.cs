using System.Web.Http;

namespace Example.HealthChecks.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Adding the health check endpoint will take place in WebApiConfig.Register.
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
