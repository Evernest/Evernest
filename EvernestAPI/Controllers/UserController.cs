using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

using EvernestAPI.Models;
using EvernestFront;


namespace EvernestAPI.Controllers
{
    public class UserController : ApiController
    {

        //     * GET /User
        // Get informations on the user represented by the user key that you must provide.
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetUser()
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("userkey"))
                return Response.MissingArgument(Request, "UserKey");

            var userProvider = new UserProvider();
            var userRequest = userProvider.GetUser((string) body["userkey"]);

            if (!userRequest.Success)
                return Response.BadArgument(Request, "UserKey");

            var user = userRequest.Result;

            var userHashtbl = new Hashtable();

            userHashtbl["Id"] = user.Id;
            userHashtbl["Name"] = user.Name;

            var userSources = new List<Hashtable>();
            foreach (var id in user.Sources)
            {
                var tmp = new Hashtable();
                tmp["Id"] = id;
                userSources.Add(tmp);
            }
            userHashtbl["Sources"] = userSources; // Possible with a Linq request ?

            var userStreams = new List<Hashtable>();
            foreach (var id in user.RelatedEventStreams)
            {
                var tmp = new Hashtable();
                tmp["Id"] = id;
                userStreams.Add(tmp);
            }
            userHashtbl["Streams"] = userStreams;

            var ans = new Hashtable();
            ans["User"] = userHashtbl;

            return Response.Success(Request, ans);
        }


        //     GET /User/{UserId}
        // Get informations on the user represented by its id.
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage GetUser(long id)
        {
            var userProvider = new UserProvider();
            var userPublicInfoRequest = userProvider.GetUserPublicInfo(id);

            if (!userPublicInfoRequest.Success)
                return Response.BadArgument(Request, "UserId");

            var userPublicInfo = userPublicInfoRequest.Result;

            var userHashtbl = new Hashtable();
            userHashtbl["Id"] = userPublicInfo.Id;
            userHashtbl["Name"] = userPublicInfo.Name;

            var ans = new Hashtable();
            ans["User"] = userHashtbl;

            return Response.Success(Request, ans);
        }

        // API can't create a User.
    }
}