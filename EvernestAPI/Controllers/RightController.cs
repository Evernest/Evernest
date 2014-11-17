using System;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class RightController : ApiController
    {
        // /Right/{id}/{streamId}
        [HttpGet]
        [HttpPost]
        public String Get(int id, int streamId)
        {
            return String.Format("/Right/{0}/{1}", id, streamId);
        }

        // /Right/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public String Set(int id, int streamId, string right)
        {
            return String.Format("/Right/{0}/{1}/Set/{2}", id, streamId, right);
        }
    }
}