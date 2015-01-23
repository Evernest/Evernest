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
        //     GET /Source
        // Get some informations on the source corresponding to the given key.
        // You need to provide a key in field Key.
        // You will get your source key, source name, your source id,
        // some informations about the user,
        // and a list of related [streams][Strea], with their id and their right.
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetSource()
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var sourceProvider = new SourceProvider();
            var sourceRequest = sourceProvider.GetSource((string) body["key"]);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "Key");

            var source = sourceRequest.Result;

            var sourceHashtbl = new Hashtable();

            sourceHashtbl["Key"] = source.Key;
            sourceHashtbl["Name"] = source.Name;
            sourceHashtbl["Id"] = source.Id;

            var userHashtbl = new Hashtable();
            userHashtbl["Id"] = source.UserId;
            sourceHashtbl["User"] = userHashtbl;

            sourceHashtbl["Streams"] = source.RelatedEventStreams;

            var ans = new Hashtable();
            ans["Source"] = sourceHashtbl;

            return Response.Success(Request, ans);
        }


        //     * GET /Source/{SourceId}
        // Same as GET /Source, but you must provide a user key, and your source id.
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetSource(long sourceId)
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("userkey"))
                return Response.MissingArgument(Request, "UserKey");

            var userProvider = new UserProvider();
            var userRequest = userProvider.GetUser((string) body["UserKey"]);

            if (!userRequest.Success)
                return Response.BadArgument(Request, "UserKey");

            var user = userRequest.Result;

            var sourceRequest = user.GetSource(sourceId);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "SourceId");

            var source = sourceRequest.Result;

            var sourceHashtbl = new Hashtable();

            sourceHashtbl["Key"] = source.Key;
            sourceHashtbl["Name"] = source.Name;
            sourceHashtbl["Id"] = source.Id;

            var userHashtbl = new Hashtable();
            userHashtbl["Id"] = source.UserId;
            sourceHashtbl["User"] = userHashtbl;

            sourceHashtbl["Streams"] = source.RelatedEventStreams;

            var ans = new Hashtable();
            ans["Source"] = sourceHashtbl;

            return Response.Success(Request, ans);
        }

        
        //     * POST /Source/New/
        // Creates a new source. You need to give your user key and a source name in field SourceName.
        [HttpGet]
        [HttpPost]
        [ActionName("New")]
        public HttpResponseMessage New()
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("userkey"))
                return Response.MissingArgument(Request, "UserKey");

            var userProvider = new UserProvider();
            var userRequest = userProvider.GetUser((string) body["userkey"]);

            if (!userRequest.Success)
                return Response.BadArgument(Request, "UserKey");

            var user = userRequest.Result;

            if (!body.ContainsKey("sourcename"))
                return Response.MissingArgument(Request, "SourceName");

            var createSourceRequest = user.CreateSource((string) body["sourcename"]);

            if (!createSourceRequest.Success)
                return Response.BadArgument(Request, "SourceName");

            var createSource = createSourceRequest.Result;
            
            var ans = new Hashtable();

            ans["Guid"] = createSource.Item2;
            
            var source = new Hashtable();
            source["Key"] = createSource.Item1;
            source["Name"] = (string) body["sourcename"];
            ans["Source"] = source;

            return Response.Success(Request, ans);
        }
    }
}