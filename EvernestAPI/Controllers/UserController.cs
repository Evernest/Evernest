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


        [HttpGet]
        [HttpPost]
        [ActionName("AddUser")]
        public Hashtable AddUser(string username, string password)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();
            Answer.AddUser user = User.AddUser(username, password);

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "User";
            debug["Method"] = "Default";
            debug["name"] = username;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            if (user.Success)
            {
                ans["Status"] = "Success";
                ans["id"] = user.id;
                ans["name"] = username;
                ans["password"] = password;
                ans["key"] = user.key;
            }
            else
            {
                ans["Status"] = "Error";
                ans["FieldErrors"] = user.Error;

            }
            return ans;
        }

        [HttpGet]
        [HttpPost]
        [ActionName("GetUser")]
        public Hashtable GetUser(long userId)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();
            Answer.GetUser user = User.GetUser(userId);

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "User";
            debug["Method"] = "GetUser";
            debug["id"] = userId;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            if (user.Success)
            {
                ans["Status"] = "Success";
                ans["key"] = user.User.key;
                ans["id"] = userId;
                ans["name"] = user.User.name;
                ans["OwnedSources"] = user.User.Sources;
                ans["RelatedStreams"] = user.User.RelatedStreams;
            }
            else
            {
                ans["Status"] = "Error";
                ans["FieldError"] = user.Error;
            }
            return ans;
        }


    }
}
