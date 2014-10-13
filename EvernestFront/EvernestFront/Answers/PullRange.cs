using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    class PullRange:Answer
    {
        //TODO : tableau d'events ? liste ? chaîne JSON ?
        List<Event> events;

        public PullRange(Exception exn)
            :base(exn)
        {
        }
        public PullRange(List<Event> eventsPulled)
        {
            success = true;
            events = eventsPulled;
        }
    }
}
