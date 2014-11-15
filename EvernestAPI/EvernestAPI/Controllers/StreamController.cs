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
        // GET: /Stream/{streamId}
        public Hashtable Get(int streamId)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "StreamController";
            response["streamId"] = streamId;
            response["action"] = "Get";
            return response;
        }
        
        // GET: /Stream/{streamId}/Pull/{random}
        public Hashtable Get(int streamId, string random)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "StreamController";
            response["streamId"] = streamId;
            response["action"] = "Get";
            response["random"] = random;
            return response;
        }
        
        // GET: /Stream/{streamId}/Pull/{id}
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

        // GET: /Stream/{streamId}/Pull/{startId}/{stopId}
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

        // POST: /Stream/{streamId}/Push
        public Hashtable Post(int streamId)
        {
            var response = new Hashtable();
            response["method"] = "POST";
            response["controller"] = "StreamController";
            response["streamId"] = streamId;
            response["action"] = "Post";
            response["notes"] = "I don't know how to read JSon body. Yet.";
            return response;
        }
    }
}
