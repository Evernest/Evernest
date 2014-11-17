using System;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class UserRightController : ApiController
    {
        // /UserRight/{id}/{streamId}
        [HttpGet]
        [HttpPost]
        public String Get(int id, int streamId)
        {
            return String.Format("/UserRight/{0}/{1}", id, streamId);
        }

        // /UserRight/{id}/{streamId}/Set/{right}
        [HttpGet]
        [HttpPost]
        [ActionName("Set")]
        public String Set(int id, int streamId, string right)
        {
            return String.Format("/UserRight/{0}/{1}/Set/{2}", id, streamId, right);
        }
    }
}