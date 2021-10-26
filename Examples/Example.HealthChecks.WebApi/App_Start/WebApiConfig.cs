using RockLib.HealthChecks.WebApi;
using System.Web.Http;

namespace Example.HealthChecks.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Add the health endpoint. The health check runner is defined in Web.config.
            config.Routes.MapHealthRoute(route: "/health", indent: true);
        }
    }
}
