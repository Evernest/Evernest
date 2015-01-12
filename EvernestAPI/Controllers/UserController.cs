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
        public HttpResponseMessage GetUser(long id)
        {
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                var user = EvernestFront.User.GetUser(id);

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "User";
                debug["Method"] = "GetUser";
                debug["id"] = id;
                debug["body"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                if (user.Success)
                {
                    ans["Status"] = "Success";
                    //ans["key"] = user.User.Key;
                    //TODO: handle removal of this field
                    ans["id"] = id;
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
        

        // User/Add/{username}/{password}
        [HttpGet]
        [HttpPost]
        [ActionName("Add")]
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



        // Not routed ?
        

    }
}