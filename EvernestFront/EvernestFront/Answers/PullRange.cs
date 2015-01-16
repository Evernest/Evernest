﻿using System.Collections.Generic;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class PullRange:Answer
    {
        //TODO : tableau d'events ? liste ? chaîne JSON ?
        public List<Event> Events { get; private set; }

        internal PullRange(FrontError err)
            :base(err)
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
