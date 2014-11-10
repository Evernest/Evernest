using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EvernestWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "StreamApiIndex",
                url: "Api/Stream",
                defaults: new { controller = "Stream", action = "Index" }
            );

            routes.MapRoute(
                name: "StreamApiPullRandom",
                url: "Api/Stream/{streamId}/Pull",
                defaults: new { controller = "Stream", action = "PullRandom" },
                constraints: new RouteValueDictionary { { "streamId", @"\d+" } }
            );

            routes.MapRoute(
                name: "StreamApiPullRange",
                url: "Api/Stream/{streamId}/Pull/{firstEventId}/{lastEventId}",
                defaults: new { controller = "Stream", action = "PullRange" },
                constraints: new RouteValueDictionary { { "streamId", @"\d+" }, { "firstEventId", @"\d+" }, { "lastEventId", @"\d+" } }
            );

            routes.MapRoute(
                name: "StreamApi",
                url: "Api/Stream/{streamId}/{action}",
                defaults: new { controller = "Stream", action = "Show" },
                constraints: new RouteValueDictionary { { "streamId", @"\d+" } }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
