using System.Collections;
using System.Web;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class RightController : ApiController
    {
        // /Right/{id}/{streamId}
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public Hashtable Get(int id, int streamId)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Right";
            debug["Method"] = "Get";
            debug["id"] = id;
            debug["streamId"] = streamId;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            return ans;
        }

        // /Right/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public Hashtable Set(int id, int streamId, string right)
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var ans = new Hashtable();

            // BEGIN DEBUG //
            var debug = new Hashtable();
            debug["Controller"] = "Right";
            debug["Method"] = "Set";
            debug["id"] = id;
            debug["streamId"] = streamId;
            debug["right"] = right;
            debug["nvc"] = nvc;
            ans["Debug"] = debug;
            // END DEBUG //

            return ans;
        }
    }
}