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
                routeTemplate: "{controller}/{streamId}/{action}",
                constraints: new {},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetRandom",
                routeTemplate: "{controller}/{streamId}/{action}/{random}",
                constraints: new {random = @"Random"},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetOne",
                routeTemplate: "{controller}/{streamId}/{action}/{id}",
                constraints: new {id = @"\d+"},
                defaults: new {controller = "Stream"}
                );

            config.Routes.MapHttpRoute(
                name: "StreamGetRange",
                routeTemplate: "{controller}/{streamId}/{action}/{startId}/{stopId}",
                constraints: new {startId = @"\d+", stopId = @"\d+"},
                defaults: new {controller = "Stream"}
                );
        }
    }
}
