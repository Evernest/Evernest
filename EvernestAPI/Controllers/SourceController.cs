using System;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class SourceController : ApiController
    {
        // /Source/{id}
        [HttpGet]
        [HttpPost]
        public String Default(int id)
        {
            return String.Format("/Source/{0}", id);
        }

        // /Source/New
        [HttpGet]
        [HttpPost]
        [ActionName("New")]
        public String New()
        {
            return String.Format("/Source/New");
        }
    }
}