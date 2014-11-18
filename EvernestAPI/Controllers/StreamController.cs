using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using EvernestFront;

namespace EvernestAPI.Controllers
{
    public class StreamController : ApiController
    {

        // /Stream/{id}
        [HttpGet]
        [HttpPost]
        public string Default(int id)
        {
            return String.Format("/Stream/{0}", id);
        }

        // /Stream/{id}/Pull/{arg0}
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public Hashtable PullOne(int id, int arg0)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();
            var key = nvc["key"];
            if (key == null)
                {
                    ans["Status"] = "Error";
                    ans["FieldErrors"] = new List<string> {"Key"};
                }
            else
                {
                    var eve = Process.Pull(key, arg0);
                    ans["Status"] = "Success";
                    ans["Events"] = new List<Event> {eve};
                };
            return ans;
        }

        // /Stream/{id}/Pull/{arg0}/{arg1}
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public string PullRange(int id, int arg0, int arg1)
        {
            return String.Format("/Stream/{0}/Pull/{1}/{2}", id, arg0, arg1);
        }

        // /Stream/{id}/Pull/Random
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public Hashtable PullRandom(int id)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();
            var key = nvc["key"];
            if (key == null)
            {
                ans["Status"] = "Error";
                ans["FieldErrors"] = new List<string> { "Key" };
            }
            else
            {
                var eve = Process.PullRandom(key);
                ans["Status"] = "Success";
                ans["Events"] = new List<Event> { eve };
            };
            return ans;
        }

        // /Stream/{id}/Push
        [HttpGet]
        [HttpPost]
        [ActionName("Push")]
        public string Push(int id)
        {
            return String.Format("/Stream/{0}/Push", id);
        }

    }
}
