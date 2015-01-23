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

        //     GET /Right/{streamId}
        // Get the rights that the Key has on the given stream.
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetRight(long streamId)
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

            var eventStreamRequest = source.GetEventStream(streamId);

            if (!eventStreamRequest.Success)
                return Response.BadArgument(Request, "StreamId");

            var eventStream = eventStreamRequest.Result;

            var right = eventStream.SourceRight;

            var ans = new Hashtable();
            ans["Right"] = AccessRightTools.AccessRightToString(right);

            return Response.Success(Request, ans);
        }


        //     * GET /Right/{SourceId}/{StreamId}
        // Get the rights the the source has on the stream. You must provide a user key.
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetRight(long sourceId, long streamId)
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


        //     * POST /Right/{sourceId}/{streamId}/Set/{right}
        // Set the right of the given source on the given stream. You must provide a user key.
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(long sourceId, long streamId, string right)
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