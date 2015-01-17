
﻿namespace EvernestFront.Responses
{
    public class PullRandomResponse : BaseResponse
    {
        public Event EventPulled {get; private set;}

        internal PullRandomResponse(FrontError err)
        :base(err)
        {
        }


        /// <summary>
        /// Sets field success to true and field eventPulled to evt.
        /// </summary>
        /// <param name="evt"></param>
        internal PullRandomResponse(Event evt)
            :base()
        {
            EventPulled = evt;
        }

         

    }
}
