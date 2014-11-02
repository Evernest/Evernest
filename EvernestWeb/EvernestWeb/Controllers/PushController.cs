using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EvernestFront;
using EvernestFront.Answers;

namespace EvernestWeb.Controllers
{
    public class PushController : ApiController
    {
        // GET api/pull
        // Get a random result
        //public PushRandom Get()
        //{
        //    PullRandom ans = EvernestFront.Process.PullRandom("user", "stream");
        //    // TODO: check that it worked
        //    return ans;
        //}

        //// GET api/pull/5
        //public PushRange Get(int id)
        //{
        //    PullRange ans = EvernestFront.Process.PullRange("user", "stream", id, id);
        //    // TODO: check that it worked
        //    return ans;
        //}

        // GET api/pull/5/10
        public PullRange Get(int begin, int end)
        {
            PullRange ans = EvernestFront.Process.PullRange("user", "stream", begin, end);
            // TODO: check that it worked
            return ans;
        }
        // POST api/pull
        public void Post([FromBody]string value)
        {
        }

        // PUT api/pull/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/pull/5
        public void Delete(int id)
        {
        }
    }
}