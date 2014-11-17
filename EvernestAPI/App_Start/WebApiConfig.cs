using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace EvernestAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // In order to use JsonFormatter for API's output.
            // We'll have to support content-type application/json. <-- TODO
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));


            /**
             * Stream
             */

            config.Routes.MapHttpRoute(
                name: "StreamGet",
                routeTemplate: "{controller}/{streamId}",
                constraints: new {},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetRandom",
                routeTemplate: "{controller}/{streamId}/Pull/{random}",
                constraints: new {random = @"Random"},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetOne",
                routeTemplate: "{controller}/{streamId}/Pull/{id}",
                constraints: new {id = @"\d+"},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetRange",
                routeTemplate: "{controller}/{streamId}/Pull/{startId}/{stopId}",
                constraints: new {startId = @"\d+", stopId = @"\d+"},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamPostEvent",
                routeTemplate: "{controller}/{streamId}/Push", // Effectivement, c'est la même que StreamGet
                constraints: new {},
                defaults: new {controller = "Stream"}
                );

            
            /**
             * Source
             */

            config.Routes.MapHttpRoute(
                name: "SourceGet",
                routeTemplate: "{controller}/{sourceId}",
                constraints: new {},
                defaults: new {controller = "Source"}
                );

            config.Routes.MapHttpRoute(
                name: "SourceNew",
                routeTemplate: "{controller}/New",
                constraints: new {},
                defaults: new {controller = "Source"}
                );

            /**
             * User
             */

            config.Routes.MapHttpRoute(
                name: "UserGet",
                routeTemplate: "{controller}/{userId}",
                constraints: new {},
                defaults: new {controller = "User"}
                );

            /**
             * Right
             */

            config.Routes.MapHttpRoute(
                name: "RightGet",
                routeTemplate: "{controller}/{sourceId}/{streamId}",
                constraints: new { },
                defaults: new { controller = "Right" }
                );

            config.Routes.MapHttpRoute(
                name: "RightSet",
                routeTemplate: "{controller}/{sourceId}/{streamId}/set/{right}",
                constraints: new {right = @"None|ReadOnly|WriteOnly|ReadWrite|Admin"},
                defaults: new {controller = "Right"}
                );

            /**
             * UserRight
             */

            config.Routes.MapHttpRoute(
                name: "UserRightGet",
                routeTemplate: "{controller}/{userId}/{streamId}",
                constraints: new { },
                defaults: new { controller = "UserRight" }
                );

            config.Routes.MapHttpRoute(
                name: "UserRightSet",
                routeTemplate: "{controller}/{userId}/{streamId}/set/{right}",
                constraints: new {right = @"None|ReadOnly|WriteOnly|ReadWrite|Admin"},
                defaults: new {controller = "UserRight"}
                );
        }
    }
}
