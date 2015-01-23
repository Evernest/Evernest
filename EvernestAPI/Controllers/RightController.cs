using System.Collections;
using System.Web.Http;
using System.Net.Http;
using EvernestAPI.Models;
using EvernestFront;
using EvernestFront.Contract;

namespace EvernestAPI.Controllers
{
    public class RightController : ApiController
    {
        // Default controller. Return the right of the source on the stream.
        //     /Right/{id}/{streamId}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Get(long sourceId, long streamId)
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

            var sourceRequest = user.GetSource(sourceId);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "SourceId");

            var source = sourceRequest.Result;

            var eventStreamRequest = source.GetEventStream(streamId);

            if (!eventStreamRequest.Success)
                return Response.BadArgument(Request, "StreamId");

            var eventStream = eventStreamRequest.Result;

            var right = eventStream.SourceRight;

            var ans = new Hashtable();
            ans["Right"] = AccessRightTools.AccessRightToString(right);

            return Response.Success(Request, ans);
        }


        // Controller to set a right.
        //     /Right/{sourceId}/{streamId}/Set/{right}
        // >>>>> userKey <<<<<
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(long sourceId, long streamId, string right)
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

            var accessRight = AccessRightTools.StringToAccessRight(right);

            var guidRequest = user.SetSourceRight(sourceId, streamId, accessRight);

            if (!guidRequest.Success)
                return Response.Error(Request, "Error while setting source right.");

            var ans = new Hashtable();
            ans["Guid"] = guidRequest.Result;
            return Response.Success(Request, ans);
        }
    }
}