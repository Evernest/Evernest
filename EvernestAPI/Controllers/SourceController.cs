using System.Collections;
using System.Web.Http;
using EvernestAPI.Models;
using System.Net;
using System.Net.Http;
using EvernestFront;
using System.Collections.Generic;
using EvernestFront.Contract;

namespace EvernestAPI.Controllers
{
    public class SourceController : ApiController
    {
        // Default controller. Get informations on the source.
        //     /Source/{id}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Default(long id)
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var sourceProvider = new SourceProvider();
            var sourceRequest = sourceProvider.GetSource((string)body["key"]);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "Key");
            
            var source = sourceRequest.Result;

            var ans = new Hashtable();
            ans["Source"] = source;

            return Response.Success(Request, ans);
        }

        // Controller that creates a new source.
        //     /Source/New/
        // >>>>>  userKey needed  <<<<<
        [HttpGet]
        [HttpPost]
        [ActionName("New")]
        public HttpResponseMessage New()
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var userProvider = new UserProvider();
            var userRequest = userProvider.GetUser((string)body["key"]);

            if (!userRequest.Success)
                return Response.BadArgument(Request, "Key");

            var user = userRequest.Result;

            var createSourceRequest = user.CreateSource((string) body["sourceName"]);

            if (!createSourceRequest.Success)
                return Response.Error(Request, "Error while creating source"); // TODO: change this

            var createSource = createSourceRequest.Result;
            var source = new Hashtable();
            source["Name"] = createSource.Item1;
            source["Guid"] = createSource.Item2; // TODO: Cannot cast expression of type 'System.Guid' to type 'long'

            var ans = new Hashtable();
            ans["Source"] = source;

            return Response.Success(Request, ans);
        }
    }
}