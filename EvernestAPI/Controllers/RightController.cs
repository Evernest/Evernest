using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class RightController : ApiController
    {
        // GET: /Right/{sourceId}/{streamId}
        public Hashtable Get(int sourceId, int streamId)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "RightController";
            response["sourceId"] = sourceId;
            response["streamId"] = streamId;
            response["action"] = "Get";
            return response;
        }

        // GET: /Right/{sourceId}/{streamId}/Set/{right}
        public Hashtable Get(int sourceId, int streamId, string right)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "RightController";
            response["action"] = "Get";
            response["sourceId"] = sourceId;
            response["streamId"] = streamId;
            response["right"] = right;
            return response;
        }
    }
}