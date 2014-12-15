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
                try
                {
                    var key = (string)body["Key"];
                    var user = EvernestFront.User.IdentifyUser(key).User;


                }
                catch
                {
                    var nokey = new Hashtable();
                    nokey["Controller"] = "Source";
                    nokey["Method"] = "New";
                    nokey["nvc"] = body;
                    nokey["Error"] = "KeyNotFound";
                    return Request.CreateResponse(HttpStatusCode.OK, nokey);
                }

                

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "Source";
                debug["Method"] = "New";
                debug["nvc"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}