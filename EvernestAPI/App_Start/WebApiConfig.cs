using System.Net.Http.Headers;
using System.Web.Http;

namespace EvernestAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // In order to use JsonFormatter for API's output.
            // We'll have to support content-type application/json. <-- TODO
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Routes.MapHttpRoute(
                name: "APIDefault",
                routeTemplate: "{controller}/{id}/{action}/{arg0}/{arg1}",
                constraints: new
                {
                    id = @"\d+",
                    action = @"[a-zA-Z]*" // Note the star to make action optional
                },
                defaults: new
                {
                    action = "Default",
                    arg0 = RouteParameter.Optional,
                    arg1 = RouteParameter.Optional
                }
                );

            config.Routes.MapHttpRoute(
                name: "APIRight",
                routeTemplate: "{controller}/{id}/{streamId}/{action}/{right}",
                constraints: new
                {
                    id = @"\d+",
                    streamId = @"\d+",
                },
                defaults: new
                {
                    action = "Get",
                    right = RouteParameter.Optional
                }
                );

            /**
             * Particular cases
             */

            config.Routes.MapHttpRoute(
                name: "APISourceNew",
                routeTemplate: "{controller}/New",
                constraints: new {},
                defaults: new
                {
                    action="New"
                }
                );
        }
    }
}
