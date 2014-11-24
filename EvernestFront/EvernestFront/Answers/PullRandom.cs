using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class PullRandom : Answer
    {
        public Event EventPulled {get; private set;}

        internal PullRandom(FrontError err)
        :base(err)
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
