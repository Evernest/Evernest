﻿using System;
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
        List<Event> events;
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="exn"></param>
        internal PullRange(FrontException exn)
            :base(exn)
        {
        }


        /// <summary>
        /// Sets field success to true and fills field events.
        /// </summary>
        /// <param name="eventsPulled"></param>
        internal PullRange(List<Event> eventsPulled)
        {
            Success = true;
            events = eventsPulled;
        }
    }
}
