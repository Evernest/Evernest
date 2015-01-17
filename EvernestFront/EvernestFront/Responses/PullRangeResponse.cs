using System.Collections.Generic;

namespace EvernestFront.Responses
{
    public class PullRangeResponse:BaseResponse
    {

        public List<Event> Events { get; private set; }

        internal PullRangeResponse(FrontError err)
            :base(err)
        {
        }


        /// <summary>
        /// Sets field success to true and fills field events.
        /// </summary>
        /// <param name="eventsPulled"></param>
        internal PullRangeResponse(List<Event> eventsPulled)
            :base ()
        {
            Events = eventsPulled;
        }
    }
}
