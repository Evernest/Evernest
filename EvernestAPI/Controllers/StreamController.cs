using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using EvernestFront;
using EvernestAPI.Models;
using System.Net;
using System.Net.Http;

namespace EvernestAPI.Controllers
{
    public class StreamController : ApiController
    {

        // /Stream/{id}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public Hashtable Default(int id)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Stream";
            debug["Method"] = "Default";
            debug["id"] = id;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            return ans;
        }

        // /Stream/{id}/Pull/{arg0}
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public Hashtable PullOne(int id, int arg0)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Stream";
            debug["Method"] = "PullOne";
            debug["id"] = id;
            debug["arg0"] = arg0;
            debug["nvc"] = nvc;            
            ans["Debug"] = debug;
            // END DEBUG //

            var key = nvc["key"];
            if (key == null)
                {
                    ans["Status"] = "Error";
                    ans["FieldErrors"] = new List<string> {"Key"};
                }
            else
            {
                var getSource = Source.GetSource(key);
                if (!getSource.Success)
                    throw new NotImplementedException();
                var pullAnswer = getSource.Source.Pull(arg0);
                if (!pullAnswer.Success)
                    throw new NotImplementedException();
                var eve = pullAnswer.EventPulled; //eve is not null at this point
                ans["Status"] = "Success";
                ans["Events"] = new List<Event> {eve};
            };
            return ans;
        }

        // /Stream/{id}/Pull/{arg0}/{arg1}
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public Hashtable PullRange(int id, int arg0, int arg1)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Stream";
            debug["Method"] = "PullRange";
            debug["id"] = id;
            debug["arg0"] = arg0;
            debug["arg1"] = arg1;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            var key = nvc["key"];
            if (key == null)
            {
                ans["Status"] = "Error";
                ans["FieldErrors"] = new List<string> { "Key" };
            }
            else
            {
                var getSource = Source.GetSource(key);
                if (!getSource.Success)
                    throw new NotImplementedException();
                var pullRange = getSource.Source.PullRange(arg0, arg1);
                if (!pullRange.Success)
                    throw new NotImplementedException();
                ans["Status"] = "Success";
                ans["Events"] = pullRange.Events;
            }
            return ans;
        }

        // /Stream/{id}/Pull/Random
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public Hashtable PullRandom(int id)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();            
            
            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Stream";
            debug["Method"] = "PullRandom";
            debug["id"] = id;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //
            
            var key = nvc["key"];
            if (key == null)
            {
                ans["Status"] = "Error";
                ans["FieldErrors"] = new List<string> { "Key" };
            }
            else
            {
                var getSource = Source.GetSource(key);
                if (!getSource.Success)
                    throw new NotImplementedException();
                var pullRandomAnswer = getSource.Source.PullRandom();
                if (!pullRandomAnswer.Success)
                    throw new NotImplementedException();
                var eve = pullRandomAnswer.EventPulled; //not null
                ans["Status"] = "Success";
                ans["Events"] = new List<Event> { eve };
            };
            return ans;
        }

        // /Stream/{id}/Push
        [HttpGet]
        [HttpPost]
        [ActionName("Push")]
        public HttpResponseMessage Push(int id)
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
