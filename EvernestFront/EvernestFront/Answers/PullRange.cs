using System.Collections.Generic;

namespace EvernestFront.Answers
{
    public class PullRange:Answer
    {

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
