using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EvernestWebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "Api/{controller}/{id}/{id2}",
                constraints: new { id = @"\d+", id2 = @"\d+"},
                defaults: new { id = RouteParameter.Optional, id2 = RouteParameter.Optional }
            );
        }
    }
}
