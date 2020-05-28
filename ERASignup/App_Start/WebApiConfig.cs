using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ERASignup
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
                routeTemplate: "{controller}/{Action}",
                defaults: new { user_site_slug = RouteParameter.Optional, id = RouteParameter.Optional }
            );
        }
    }
}
