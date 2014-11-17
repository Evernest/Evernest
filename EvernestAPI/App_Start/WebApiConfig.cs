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
                name: "APIRoute",
                routeTemplate: "{controller}/{id}/{action}/{arg0}/{arg1}",
                constraints: new {},
                defaults: new {action = "Default", arg0 = RouteParameter.Optional, arg1 = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute(
                name: "RightsRoute",
                routeTemplate: "{controller}/{id}/{streamId}/{action}/{right}",
                constraints: new {right = @"None|ReadOnly|WriteOnly|ReadWrite|Admin"},
                defaults: new {action="Get", right=RouteParameter.Optional}
                );
        }
    }
}
