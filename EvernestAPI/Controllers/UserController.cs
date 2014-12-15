using System.Collections;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using EvernestAPI.Models;

namespace EvernestAPI.Controllers
{
    public class UserController : ApiController
    {
        // /User/{id}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Default(int id)
        {

            Hashtable nvc;
            var ans = new Hashtable();

            try
            {
                nvc = Tools.ParseRequest(Request);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);   
            }

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "User";
            debug["Method"] = "Default";
            debug["id"] = id;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            return Request.CreateResponse(HttpStatusCode.OK, ans);
        }


        [HttpGet]
        [HttpPost]
        [ActionName("AddUser")]
        public Hashtable AddUser(string username, string password)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();
            EvernestFront.Answers.AddUser user = EvernestFront.User.AddUser(username, password);

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
                ans["id"] = user.UserId;
                ans["name"] = username;
                ans["password"] = password;
                ans["key"] = user.UserKey;
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
            EvernestFront.Answers.GetUser user = EvernestFront.User.GetUser(userId);

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
                //ans["key"] = user.User.Key;
                //TODO: handle removal of this field
                ans["id"] = userId;
                ans["name"] = user.User.Name;
                ans["OwnedSources"] = user.User.Sources;
                ans["RelatedStreams"] = user.User.RelatedEventStreams;
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
