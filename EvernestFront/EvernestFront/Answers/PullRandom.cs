
﻿namespace EvernestFront.Answers
{
    public class PullRandom : Answer
    {
        public Event EventPulled {get; private set;}

        internal PullRandom(FrontError err)
        :base(err)
        {
        }


        /// <summary>
        /// Sets field success to true and field eventPulled to evt.
        /// </summary>
        /// <param name="evt"></param>
        internal PullRandom(Event evt)
            :base()
        {
            EventPulled = evt;
        }

         

    }
}
