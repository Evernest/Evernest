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
        // /Right/{id}/{streamId}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Get(string sourceId, int streamId)
        {
            var ans = new Hashtable();

            try
            {
                body = Tools.ParseRequest(Request);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Get the Stream
            var front = new EvernestFront.UsersBuilder();
            var userReq = front.GetUser((string)body["key"]);

            if (!userReq.Success)
            {
                ans["Status"] = "Error";
                ans["Error"] = userReq.Error;
                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }

            var user = userReq.Result;


            var sourceReq = user.GetSource(sourceId);
            if (!sourceReq.Success)
            {
                ans["Status"]="Error";
                ans["Error"]=sourceReq.Error;
                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            var source = sourceReq.Result;

            var eventStreamReq = source.GetEventStream(streamId);

                if (!eventStreamReq.Success)
                {
                    ans["Status"] = "Error";
                    ans["Error"] = eventStreamReq.Error;
                    return Request.CreateResponse(HttpStatusCode.OK, ans);
                }

                var stream = eventStreamReq.Result;
      
               var accessRight = stream.UserRight;

               ans["AccessRight"] = accessRight;
            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Right";
            debug["Method"] = "Set";
            debug["sourceId"] = sourceId;
            debug["streamId"] = streamId;
            debug["right"] = accessRight;
            debug["body"] = body;
            ans["Debug"] = debug;
            // END DEBUG //

            return Request.CreateResponse(HttpStatusCode.OK, ans);
        }

        // /Right/{sourceId}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(string sourceId, int streamId, string right)
        {
            var ans = new Hashtable();
<<<<<<< HEAD
            Hashtable body;
=======
            Hashtable nvc;
            var failed = false;

            FrontError? error = null;
            string errorMessage = null;
            var accessRight = AccessRight.NoRight;

>>>>>>> origin/master
            try
            {
                body = Tools.ParseRequest(Request);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Get the Stream
            var front = new EvernestFront.UsersBuilder();
            var userReq = front.GetUser((string)body["key"]);

            if (!userReq.Success)
            {
                ans["Status"] = "Error";
                ans["Error"] = userReq.Error;
                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }

            var user = userReq.Result;


            var sourceReq = user.GetSource(sourceId);
            if (!sourceReq.Success)
            {
                ans["Status"] = "Error";
                ans["Error"] = sourceReq.Error;
                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            var source = sourceReq.Result;

            var eventStreamReq = source.GetEventStream(streamId);

            if (!eventStreamReq.Success)
            {
                ans["Status"] = "Error";
                ans["Error"] = eventStreamReq.Error;
                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }

            var stream = eventStreamReq.Result;

            AccessRight accessRight;

            switch (right.ToLower())
                    {
                        case "none":
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
                            // Should never happen
                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
            
                    // Convert the string to an AccessRights enum

            var GuidReq = stream.SetUserRight(sourceId, accessRight);

            //TODO: Check if it is really sourceId
            if (!GuidReq.Success)
            {
                ans["Status"]="Error";
                ans["Error"]=GuidReq.Error;
                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }

            var Guid = GuidReq.Result;

            ans["Guid"]=Guid;
            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Right";
            debug["Method"] = "Set";
            debug["sourceId"] = sourceId;
            debug["streamId"] = streamId;
            debug["right"] = accessRight;
            debug["body"] = body;
            ans["Debug"] = debug;
            // END DEBUG //

            return Request.CreateResponse(HttpStatusCode.OK, ans);
            
            
        }
    }
}