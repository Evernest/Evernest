using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Answers
{
    public class PullRange:Answer
    {
        //TODO : tableau d'events ? liste ? chaîne JSON ?
        public List<Event> Events { get; private set; }
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="msg"></param>
        internal PullRange(String msg)
            :base(msg)
        {
        }


        /// <summary>
        /// Sets field success to true and fills field events.
        /// </summary>
        /// <param name="eventsPulled"></param>
        internal PullRange(List<Event> eventsPulled)
            :base ()
        {
            Events = eventsPulled;
        }
    }
}
