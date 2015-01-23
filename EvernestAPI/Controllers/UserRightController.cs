using System.Collections;
using System.Net.Http;
using System.Web.Http;
using EvernestAPI.Models;
using EvernestFront;

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
            return Response.NotImplemented(Request);
        }


        
        // /UserRight/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(long id, long streamId, string right)
        {
            return Response.NotImplemented(Request);
        }
    }
}