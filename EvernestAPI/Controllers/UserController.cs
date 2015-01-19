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
        public HttpResponseMessage GetUser()
        {
            try
            {
                var body = Tools.ParseRequest(Request);
                var ans = new Hashtable();

                var front = new EvernestFront.UsersBuilder();

                var userReq = front.GetUser((string)body["key"]);

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "User";
                debug["Method"] = "Response<User>";
                debug["key"] = body["key"];
                debug["body"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                if (userReq.Success)
                {
                    var user = userReq.Result;
                    ans["Status"] = "Success";
                    ans["id"] = user.Id;
                    ans["name"] = user.Name;
                    ans["OwnedSources"] = user.Sources;
                    ans["RelatedStreams"] = user.RelatedEventStreams;
                }
                else
                {
                    ans["Status"] = "Error";
                    ans["FieldError"] = userReq.Error;
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

                var front =  new EvernestFront.UsersBuilder();


                var GuidReq = front.AddUser(username, password);

                

                // BEGIN DEBUG //
                var debug = new Hashtable();
                debug["Controller"] = "User";
                debug["Method"] = "Default";
                debug["key"] = body["key"];
                debug["body"] = body;
                ans["Debug"] = debug;
                // END DEBUG //

                if (GuidReq.Success)
                {
                    var Guid = GuidReq.Result;
                    ans["Status"] = "Success";
                    ans["Guid"] = Guid;
                }
                else
                {
                    ans["Status"] = "Error";
                    ans["FieldErrors"] = GuidReq.Error;

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