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
        public HttpResponseMessage Get(int id, int streamId)
        {
            try
            {

                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "Right";
                debug["Method"] = "Get";
                debug["id"] = id;
                debug["streamId"] = streamId;
                debug["body"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }



        // /Right/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(int id, int streamId, string right)
        {
            var ans = new Hashtable();
            Hashtable nvc;
            var failed = false;
            EvernestFront.Errors.FrontError error = null;
            var errorMessage = "";
            try
            {
                nvc = Tools.ParseRequest(Request);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            AccessRights accessRight;
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
            // Get the User
            var getUser = EvernestFront.User.GetUser(id);
            if (!getUser.Success)
            {
                failed = true;
                errorMessage = "Can't access the user";
                error = getUser.Error;
                goto end;
            }
            // Modifies the accessRights
            var answer = getUser.User.SetRights(streamId, id, accessRight);
            if (!answer.Success)
            {
                failed = true;
                errorMessage = "Can't modify rights.";
                error = answer.Error;
                // goto end;
            }

            end:

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Right";
            debug["Method"] = "Set";
            debug["id"] = id;
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