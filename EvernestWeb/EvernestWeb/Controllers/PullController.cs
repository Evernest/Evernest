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
    public class PullController : ApiController
    {
        // GET api/pull
        // Get a random result
        public PullRandom GetRandom()
        {
            PullRandom ans = EvernestFront.Process.PullRandom("user", "stream");
            // TODO: check that it worked
            return ans;
        }

        // GET api/pull/5/10
        public PullRange GetRange(int id, int id2)
        {
            PullRange ans = EvernestFront.Process.PullRange("user", "stream", id, id2);
            //// TODO: check that it worked
            return ans;
        }

        // GET api/pull/5
        public PullRange GetOne(int id)
        {
            return GetRange(id, id) ;
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