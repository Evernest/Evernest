using System.Collections;
using System.Web.Http;
using EvernestAPI.Models;
using System.Net;
using System.Net.Http;
using EvernestFront;
using System.Collections.Generic;

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
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "Stream";
                debug["Method"] = "Push";
                debug["id"] = id;
                debug["body"] = body;
                ans["Debug"] = debug;
                ans["Status"] = "Error";
                // END DEBUG //

                try
                {
                    var key = (string) body["Key"];
                    var gsource = Source.GetSource(key);
                    if (!gsource.Success)
                    {
                        var nosource = ans;
                        ans["FieldErrors"] = gsource.Source;
                        return Request.CreateResponse(HttpStatusCode.OK, nosource);
                    }
                    ans["Status"] = "Success";
                    ans["Sources"] = new List<Source> {gsource.Source};

                }
                catch
                {
                    var nokey = ans;
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

        // /Source/New/
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
                debug["body"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                ans["Status"] = "Error";

                try
                {
                    var key = (string) body["Key"];
                    var iduser = EvernestFront.User.IdentifyUser(key);
                    if (!iduser.Success)
                    {
                        ans["Error"] = "UserNotFound";
                        return Request.CreateResponse(HttpStatusCode.OK, ans);
                    }
                    var user = iduser.User;
                    var sourceName = (string) body["SourceName"];
                    var streamId = (long) body["StreamId"];
                    var rights = (AccessRight) body["AccessRights"]; // ?
                    var creaSource = user.CreateSource(sourceName, streamId, rights);
                    if (!creaSource.Success)
                    {
                        ans["Error"] = "NoSourceCreated";
                        return Request.CreateResponse(HttpStatusCode.OK, ans);
                    }
                    ans["Status"] = "Success";

                }
                catch
                {
                    ans["Error"] = "KeyNotFound";
                    return Request.CreateResponse(HttpStatusCode.OK, ans);
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