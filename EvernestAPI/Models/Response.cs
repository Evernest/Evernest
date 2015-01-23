using System.Net;
using System.Net.Http;
using System.Collections;

namespace EvernestAPI.Models
{
    internal static class Response
    {
        public static HttpResponseMessage MissingArgument(HttpRequestMessage request, string argument)
        {
            var ans = new Hashtable();

            ans["Succes"] = false;

            var error = new Hashtable();
            error["Message"] = "MissingArgument";
            error["HelpMessage"] = ""; // TODO: Fill this field.
            error["Argument"] = argument;
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.BadRequest, ans);            
        }

        public static HttpResponseMessage BadArgument(HttpRequestMessage request, string argument)
        {
            var ans = new Hashtable();

            ans["Succes"] = false;

            var error = new Hashtable();
            error["Message"] = "BadArgument";
            error["HelpMessage"] = ""; // TODO: Fill this field.
            error["Argument"] = argument;
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.BadRequest, ans);
        }

        public static HttpResponseMessage BadRequest(HttpRequestMessage request)
        {
            var ans = new Hashtable();

            ans["Success"] = false;
            
            var error = new Hashtable();
            error["Message"] = "BadRequest";
            error["HelpMessage"] = ""; // TODO: Fill this field.
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.BadRequest, ans);
        }

    }
}