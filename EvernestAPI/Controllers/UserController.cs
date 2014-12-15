using System.Collections;
using System.Net;
using System.Net.Http;
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
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "User";
                debug["Method"] = "Default";
                debug["id"] = id;
                debug["body"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }



        // Pas branché.
        [HttpGet]
        [HttpPost]
        [ActionName("AddUser")]
        public HttpResponseMessage AddUser(string username, string password)
        {
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                var user = EvernestFront.User.AddUser(username, password);

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "User";
                debug["Method"] = "Default";
                debug["name"] = username;
                debug["body"] = body;
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
                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }



        // Pas branché
        [HttpGet]
        [HttpPost]
        [ActionName("GetUser")]
        public HttpResponseMessage GetUser(long userId)
        {
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                var user = EvernestFront.User.GetUser(userId);

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "User";
                debug["Method"] = "GetUser";
                debug["id"] = userId;
                debug["body"] = body;
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

                return Request.CreateResponse(HttpStatusCode.OK, ans);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

    }
}