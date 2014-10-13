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

        public PullRandom(Exception exn)
        :base(exn)
        {
        }

        public PullRandom(Event evt)
        {
            success = true;
            eventPulled = evt;
        }

         

    }
}
