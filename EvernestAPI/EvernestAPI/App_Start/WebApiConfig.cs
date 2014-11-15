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
             * Stream Get
             */

            config.Routes.MapHttpRoute(
                name: "StreamGet",
                routeTemplate: "Stream/{streamId}",
                constraints: new {},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetRandom",
                routeTemplate: "Stream/{streamId}/Pull/{random}",
                constraints: new {random = @"Random"},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetOne",
                routeTemplate: "Stream/{streamId}/Pull/{id}",
                constraints: new {id = @"\d+"},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetRange",
                routeTemplate: "Stream/{streamId}/Pull/{startId}/{stopId}",
                constraints: new {startId = @"\d+", stopId = @"\d+"},
                defaults: new {controller = "Stream"}
                );

            /**
             * Stream Post
             */

            config.Routes.MapHttpRoute(
                name: "StreamPostEvent",
                routeTemplate: "Stream/{streamId}/Push", // Effectivement, c'est la même que StreamGet
                constraints: new {},
                defaults: new {controller = "Stream"}
                );

            /**
             * Source Get
             */

            config.Routes.MapHttpRoute(
                name: "SourceGet",
                routeTemplate: "Source/{sourceId}",
                constraints: new {},
                defaults: new {controller = "Source"}
                );

            /**
             * Source New
             */

            config.Routes.MapHttpRoute(
                name: "SourceNew",
                routeTemplate: "Source/New",
                constraints: new {},
                defaults: new {controller = "Source"}
                );

        }
    }
}
