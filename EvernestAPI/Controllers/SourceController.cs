using System.Collections;
using System.Web;
using System.Web.Http;
using EvernestAPI.Models;
using System.Net;
using System.Net.Http;
using EvernestFront;

namespace EvernestAPI.Controllers
{
    public class SourceController : ApiController
    {
        // /Source/{id}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Default(int id)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Source";
            debug["Method"] = "Default";
            debug["id"] = id;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // /Source/New
        [HttpGet]
        [HttpPost]
        [ActionName("New")]
        public HttpResponseMessage New()
        {
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();
                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "Source";
                debug["Method"] = "New";
                debug["nvc"] = body;
                ans["Debug"] = debug;
                ans["Status"] = "Error";
                // END DEBUG //
              
                try
                {
                    var key = (string)body["Key"];
                    var iduser = EvernestFront.User.IdentifyUser(key);
                    if (!iduser.Success)
                    {
                        var nouser = ans;
                        nouser["Error"] = "UserNotFound";
                        return Request.CreateResponse(HttpStatusCode.OK,nouser);
                    }
                    var user = iduser.User;
                    var sourceName = (string)body["SourceName"];
                    var streamId = (long)body["StreamId"];
                    var rights = (AccessRights)body["AccessRights"];
                    var creaSource = user.CreateSource(sourceName, streamId, rights);
                    if (!creaSource.Success)
                    {
                        var nosource = ans;
                        nosource["Error"] = "NoSourceCreated";
                        return Request.CreateResponse(HttpStatusCode.OK,nosource);
                    }
                    ans["Status"] = "Success";

                }
                catch
                {
                    var nokey = new Hashtable();
                    nokey = ans;
                    nokey["Error"] = "KeyNotFound";
                    return Request.CreateResponse(HttpStatusCode.OK, nokey);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}