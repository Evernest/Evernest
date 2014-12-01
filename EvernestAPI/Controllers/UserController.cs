using System;
using System.Collections;
using System.Web;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class UserController : ApiController
    {
        // /User/{id}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public Hashtable Default(int id)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "User";
            debug["Method"] = "Default";
            debug["id"] = id;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            return ans;
        }
    }
}
