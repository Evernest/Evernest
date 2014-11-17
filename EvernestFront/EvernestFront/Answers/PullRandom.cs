using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Answers
{
    public class PullRandom : Answer
    {
        public Event EventPulled {get; private set;}
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="msg"></param>
        internal PullRandom(String msg)
        :base(msg)
        {
        }


        /// <summary>
        /// Sets field success to true and field eventPulled to evt.
        /// </summary>
        /// <param name="evt"></param>
        internal PullRandom(Event evt)
            :base()
        {
            EventPulled = evt;
        }

         

    }
}
