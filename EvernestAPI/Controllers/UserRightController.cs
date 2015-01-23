using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EvernestAPI.Models;

namespace EvernestAPI.Controllers
{
    public class UserRightController : ApiController
    {
        // /UserRight/{id}/{streamId}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Get(long id, long streamId)
        {
        }


        
        // /UserRight/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(long id, long streamId, string right)
        {
        }
    }
}