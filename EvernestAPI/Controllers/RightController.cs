using System.Collections;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using EvernestAPI.Models;
using EvernestFront;
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
            var failed = false;
            EvernestFront.Errors.FrontError error = null;
            var errorMessage = "";
            var accessRight = new AccessRights();
            Hashtable nvc;
            try
            {
                nvc = Tools.ParseRequest(Request);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
			// Get the Stream
            var getSource = Source.GetSource(sourceId);
            if (!getSource.Success)
            {
                failed = true;
                errorMessage = "Can't access the user";
                error = getSource.Error;
            }
            else
            {
                var source = getSource.Source;
                // The source haven't access to stream
                if (source.EventStream.Id != streamId)
                {
                    accessRight = AccessRights.NoRights;
                }
                else
                {
                    accessRight = source.Right;
                }
            }
            
            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Right";
            debug["Method"] = "Set";
            debug["sourceId"] = sourceId;
            debug["streamId"] = streamId;
            debug["right"] = accessRight;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            if (!failed) return Request.CreateResponse(HttpStatusCode.OK, ans);

            ans["error"] = error;
            ans["errorMessage"] = errorMessage;

            return Request.CreateResponse(HttpStatusCode.OK, ans);
        }

        // /Right/{sourceId}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(string sourceId, int streamId, string right)
        {
            var ans = new Hashtable();
            var nvc = new Hashtable();
            bool failed = false;
            EvernestFront.User user = null;
            EvernestFront.Errors.FrontError error = null;
            string errorMessage = null;
            var accessRight = AccessRights.NoRights;
            try
            {
                nvc = Tools.ParseRequest(Request);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Get the user
            if (nvc.ContainsKey("key")) {
                var getUser = EvernestFront.User.GetUser((int)nvc["key"]);
                if (!getUser.Success)
                {
                    failed = true;
                    errorMessage = "Can't access the source.";
                    error = getUser.Error;
                }
                else
                {
                    user = getUser.User;

                    // Convert the string to an AccessRights enum
                    switch (right.ToLower())
                    {
                        case "none":
                            accessRight = AccessRights.NoRights;
                            break;
                        case "readonly":
                            accessRight = AccessRights.ReadOnly;
                            break;
                        case "writeonly":
                            accessRight = AccessRights.WriteOnly;
                            break;
                        case "readwrite":
                            accessRight = AccessRights.ReadWrite;
                            break;
                        case "admin":
                            accessRight = AccessRights.Admin;
                            break;
                        case "root":
                            accessRight = AccessRights.Root;
                            break;

                        default:
                            // Should never happen
                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }

                    // Get the Source
                    var getSource = EvernestFront.Source.GetSource(sourceId);
                    if (!getSource.Success)
                    {
                        failed = true;
                        errorMessage = "Can't access the source.";
                        error = getSource.Error;
                    }
                    else
                    {
                        // Modifies the accessRights
                        var answer = user.SetRights(streamId, user.Id, accessRight);
                        if (!answer.Success)
                        {
                            failed = true;
                            errorMessage = "Can't modify rights.";
                            error = answer.Error;
                        }
                    }
                }
            }
            else 
            {
                failed = true;
                errorMessage = "Missing user id.";
                error = null;
            }
            
           
            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Right";
            debug["Method"] = "Set";
            debug["id"] = sourceId;
            debug["streamId"] = streamId;
            debug["right"] = accessRight;
            debug["nvc"] = nvc;
            if (failed)
            {
                debug["error"] = error;
                debug["errorMessage"] = errorMessage;
                debug["error"] = error;
            }
            ans["Debug"] = debug;
            // END DEBUG //

            return Request.CreateResponse(HttpStatusCode.OK, ans);
        }
    }
}