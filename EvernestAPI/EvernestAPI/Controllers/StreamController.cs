using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class StreamController : ApiController
    {
        // GET: /Stream/{streamId}/Get
        public Hashtable Get(int streamId)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "StreamController";
            response["streamId"] = streamId;
            response["action"] = "Get";
            return response;
        }
        
        // GET: /Stream/{streamId}/Get/{random}
        public Hashtable Get(int streamId, string random)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "StreamController";
            response["streamId"] = streamId;
            response["action"] = "Get";
            return response;
        }
        
        // GET: /Stream/{streamId}/Get/{id}
        public Hashtable Get(int streamId, int id)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "StreamController";
            response["streamId"] = streamId;
            response["action"] = "Get";
            response["id"] = id;
            return response;
        }

        // GET: /Stream/{streamId}/Get/{startId}/{stopId}
        public Hashtable Get(int streamId, int startId, int stopId)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "StreamController";
            response["streamId"] = streamId;
            response["action"] = "Get";
            response["startId"] = startId;
            response["stopId"] = stopId;
            return response;
        }
    }
}
