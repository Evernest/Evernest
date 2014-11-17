using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class SourceController : ApiController
    {
        // GET: /Source/{sourceId}
        public Hashtable Get(int sourceId)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "SourceController";
            response["sourceId"] = sourceId;
            response["action"] = "Get";
            return response;
        }

        // POST: /Source/New
        public Hashtable Post()
        {
            var response = new Hashtable();
            response["method"] = "POST";
            response["controller"] = "SourceController";
            response["action"] = "Post";
            response["notes"] = "I don't know how to read JSon body. Yet.";
            return response;
        }
    }
}