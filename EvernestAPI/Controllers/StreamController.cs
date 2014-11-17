using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using EvernestFront;

namespace EvernestAPI.Controllers
{
    public class StreamController : ApiController
    {
        // GET: /Stream/{id}
        // GET: /Stream/{id}/Pull/{random}
        // GET: /Stream/{id}/Pull/{arg}
        // GET: /Stream/{id}/Pull/{arg0}/{arg1}
        
        // POST: /Stream/{streamId}/Push

        [HttpGet]
        [HttpPost]
        public Hashtable Pull(int id, int arg)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();
            if (nvc["key"] == null)
                {
                    ans["Status"] = "Error";
                    List<string> errors = new List<string>();
                    errors.Add("Key");
                    ans["FieldErrors"] = errors;
                }
            else
                {
                    string key = nvc["key"];
                    Event eve = Process.Pull(key, arg);
                    ans["Status"] = "Success";
                    List<Event> aux = new List<Event>();
                    aux.Add(eve);
                    ans["Events"] = aux;                 
                };
            return ans;
        }

        

        [HttpGet]
        [HttpPost]
        public string PullRandom(int id)
        {
            return "pull random";
        }

    }
}
