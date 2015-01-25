using System.Net.Http.Headers;
using System.Web.Http;

namespace EvernestAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Routes.MapHttpRoute(
                name: "APIStream",
                routeTemplate: "{controller}/{id}/{action}/{arg0}/{arg1}",
                constraints: new
                {
                    id = @"\d*", // Note the star to make id optional
                    action = @"[a-zA-Z]*", // Note the star to make action optional
                    arg0 = @"\d*", // Note the star to make arg0 optional
                    arg1 = @"\d*", // Note the star to make arg1 optional
                },
                defaults: new
                {
                    id = RouteParameter.Optional,
                    action = "Default",
                    arg0 = RouteParameter.Optional,
                    arg1 = RouteParameter.Optional,
                }
                );

            config.Routes.MapHttpRoute(
                name: "APIRight",
                routeTemplate: "{controller}/{id}/{streamId}/{action}/{right}",
                constraints: new
                {
                    id = @"\d+",
                    streamId = @"\d*", // Note the star to make streamId optional
                    action = @"[a-zA-Z]*", // Note the star to make action optional
                    right = @"(None|ReadOnly|WriteOnly|ReadWrite|Admin)?", // Note the ? to make right optional
                },
                defaults: new
                {
                    streamId = RouteParameter.Optional,
                    action = "Default",
                    right = RouteParameter.Optional,
                }
                );

            /**
             * Particular cases
             */

            config.Routes.MapHttpRoute(
                name: "APIStreamPullRandom",
                routeTemplate: "{controller}/{id}/{action}/Random",
                constraints: new
                {
                    id = @"\d+",
                },
                defaults: new {}
                );
          
            config.Routes.MapHttpRoute(
                name: "APISourceNew",
                routeTemplate: "{controller}/New/",
                constraints: new {},
                defaults: new
                {
                    action="New",
                }
                );
        }
    }
}
