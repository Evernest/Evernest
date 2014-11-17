using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class UserRightController : ApiController
    {
        // GET: /UserRight/{userId}/{streamId}
        public Hashtable Get(int userId, int streamId)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "UserRightController";
            response["userId"] = userId;
            response["streamId"] = streamId;
            response["action"] = "Get";
            return response;
        }

        // GET: /Right/{userId}/{streamId}/Set/{right}
        public Hashtable Get(int userId, int streamId, string right)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "UserRightController";
            response["action"] = "Get";
            response["userId"] = userId;
            response["streamId"] = streamId;
            response["right"] = right;
            return response;
        }
    }
}