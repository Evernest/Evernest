using System.Net.Http;
using System.Web.Http;
using EvernestAPI.Models;

namespace EvernestAPI.Controllers
{
    public class UserController : ApiController
    {
        //     GET /User/{UserId}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetUser(long id)
        {
            return Response.NotImplemented(Request);
        }

        // API can't create a User.
    }
}