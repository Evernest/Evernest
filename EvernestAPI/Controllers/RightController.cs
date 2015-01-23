using System.Collections;
using System.Web.Http;
using System.Net;
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
            switch (right)
            {
                case AccessRight.NoRight:
                    ans["Right"] = "NoRight";
                    break;

                case AccessRight.ReadOnly:
                    ans["Right"] = "ReadOnly";
                    break;

                case AccessRight.WriteOnly:
                    ans["Right"] = "WriteOnly";
                    break;

                case AccessRight.ReadWrite:
                    ans["Right"] = "ReadWrite";
                    break;

                case AccessRight.Admin:
                    ans["Right"] = "Admin";
                    break;

                case AccessRight.Root:
                    ans["Right"] = "Root";
                    break;
            }

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

            AccessRight accessRight;
            switch (right.ToLower())
            {
                case "noright":
                    accessRight = AccessRight.NoRight;
                    break;

                case "readonly":
                    accessRight = AccessRight.ReadOnly;
                    break;

                case "writeonly":
                    accessRight = AccessRight.WriteOnly;
                    break;

                case "readwrite":
                    accessRight = AccessRight.ReadWrite;
                    break;

                case "admin":
                    accessRight = AccessRight.Admin;
                    break;

                case "root":
                    accessRight = AccessRight.Root;
                    break;

                default:
                    return Response.BadArgument(Request, "Right");
                    break;
            }

            var guidRequest = user.SetSourceRight(sourceId, streamId, accessRight);

            if (!guidRequest.Success)
                return Response.Error(Request, "Error while setting source right.");

            var ans = new Hashtable();
            ans["Guid"] = guidRequest.Result;
            return Response.Success(Request, ans);
        }
    }
}