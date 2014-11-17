using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class StreamController : ApiController
    {
        // GET: /Stream/{id}
        // GET: /Stream/{id}/Pull/{random}
        // GET: /Stream/{id}/Pull/{id}
        // GET: /Stream/{id}/Pull/{arg0}/{arg1}
        
        // POST: /Stream/{streamId}/Push

        [HttpGet]
        [HttpPost]
        public string Pull(int id)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            return nvc["example"];
        }

        [HttpGet]
        [HttpPost]
        public int Pull(int id, int arg)
        {
            return arg;
        }

        [HttpGet]
        [HttpPost]
        public string PullRandom(int id)
        {
            return "pull random";
        }

    }
}
