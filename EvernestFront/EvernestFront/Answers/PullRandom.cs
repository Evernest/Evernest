using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    class PullRandom : Answer
    {
        Event eventPulled = null;
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="exn"></param>
        public PullRandom(Exception exn)
        :base(exn)
        {
        }


        /// <summary>
        /// Sets field success to true and field eventPulled to evt.
        /// </summary>
        /// <param name="evt"></param>
        public PullRandom(Event evt)
        {
            Success = true;
            eventPulled = evt;
        }

         

    }
}
