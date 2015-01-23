using System.Net.Http;
using System.Web.Http;
using EvernestAPI.Models;

namespace EvernestAPI.Controllers
{
    public class UserRightController : ApiController
    {
        //     GET /UserRight/{UserId}/{StreamId}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Get(long id, long streamId)
        {
            return Response.NotImplemented(Request);
        }


        
        // /UserRight/{UserId}/{StreamId}/Set/{Right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public HttpResponseMessage Set(long id, long streamId, string right)
        {
            return Response.NotImplemented(Request);
        }
    }
}