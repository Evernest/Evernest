using System.Collections;
using System.Web.Http;
using EvernestAPI.Models;

namespace EvernestAPI.Controllers
{
    public class UserRightController : ApiController
    {
        // /UserRight/{id}/{streamId}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public Hashtable Get(int id, int streamId)
        {
            var body = Tools.ParseRequest(Request);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Source";
            debug["Method"] = "Get";
            debug["id"] = id;
            debug["streamId"] = streamId;
            debug["body"] = body;
            ans["Debug"] = debug;
            // END DEBUG //

            return ans;
        }

        // /UserRight/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public Hashtable Set(int id, int streamId, string right)
        {
            var body = Tools.ParseRequest(Request);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Source";
            debug["Method"] = "Set";
            debug["id"] = id;
            debug["streamId"] = streamId;
            debug["right"] = right;
            debug["body"] = body;
            ans["Debug"] = debug;
            // END DEBUG //

            return ans;
        }
    }
}