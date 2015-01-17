
﻿namespace EvernestFront.Responses

{
    public class PullResponse : BaseResponse
    {
        public Event EventPulled {get; private set;}

        internal PullResponse(FrontError err)
        :base(err)
        {
        }


        /// <summary>
        /// Sets field success to true and field eventPulled to evt.
        /// </summary>
        /// <param name="evt"></param>
        internal PullResponse(Event evt)
            :base()
        {
            EventPulled = evt;
        }
    }
}
