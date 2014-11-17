using System;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class UserController : ApiController
    {
        // /User/{id}
        [HttpGet]
        [HttpPost]
        public String Default(int id)
        {
            return String.Format("/User/{0}", id);
        }
    }
}
