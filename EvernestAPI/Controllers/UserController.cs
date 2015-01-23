using System.Collections;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EvernestAPI.Models;
using EvernestFront;
using EvernestFront.Contract;

namespace EvernestAPI.Controllers
{
    public class UserController : ApiController
    {
        // /User/{id}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetUser(long id)
        {
            return Response.NotImplemented(Request);
        }
        

        // User/Add/{username}/{password}
        [HttpGet]
        [HttpPost]
        [ActionName("Add")]
        public HttpResponseMessage AddUser(string username, string password)
        {
            return Response.NotImplemented(Request);
        }
    }
}