using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class UserController : ApiController
    {
        // GET: /User/{userId}
        public Hashtable Get(int userId)
        {
            var response = new Hashtable();
            response["method"] = "GET";
            response["controller"] = "UserController";
            response["sourceId"] = userId;
            response["action"] = "Get";
            return response;
        }
    }
}
