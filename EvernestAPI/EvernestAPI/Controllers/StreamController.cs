using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EvernestAPI.Controllers
{
    public class StreamController : ApiController
    {
        // GET: api/Stream
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Stream/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Stream
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Stream/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Stream/5
        public void Delete(int id)
        {
        }
    }
}
