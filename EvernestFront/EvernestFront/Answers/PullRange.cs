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
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="exn"></param>
        public PullRange(Exception exn)
            :base(exn)
        {
        }


        /// <summary>
        /// Sets field success to true and fills field events.
        /// </summary>
        /// <param name="eventsPulled"></param>
        public PullRange(List<Event> eventsPulled)
        {
            success = true;
            events = eventsPulled;
        }
    }
}
