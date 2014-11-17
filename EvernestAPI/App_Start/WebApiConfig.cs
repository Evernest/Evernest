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

            // GET: /Stream/{streamId}
            // GET: /Stream/{streamId}/Pull/{random}
            // GET: /Stream/{streamId}/Pull/{id}
            // GET: /Stream/{streamId}/Pull/{startId}/{stopId}

            // POST: /Stream/{streamId}/Push

            config.Routes.MapHttpRoute(
                name: "NoArg",
                routeTemplate: "{controller}/{id}/{action}",
                constraints: new { },
                defaults: new { }
                );

            config.Routes.MapHttpRoute(
                name: "OneArg",
                routeTemplate: "{controller}/{id}/{action}/{arg}",
                constraints: new { },
                defaults: new { }
                );

            config.Routes.MapHttpRoute(
                name: "TwoArg",
                routeTemplate: "{controller}/{id}/{action}/{firstArg}/{secondArg}",
                constraints: new { },
                defaults: new { }
                );

        }
    }
}
