using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EvernestAPI.Models;

namespace EvernestAPI.Controllers
{
    public class UserRightController : ApiController
    {
        // /UserRight/{id}/{streamId}
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
                debug["Controller"] = "Source";
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


        
        // /UserRight/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(int id, int streamId, string right)
        {
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "Source";
                debug["Method"] = "Set";
                debug["id"] = id;
                debug["streamId"] = streamId;
                debug["right"] = right;
                debug["body"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}