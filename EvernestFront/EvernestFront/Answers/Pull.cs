
﻿namespace EvernestFront.Answers

{
    public class Pull : Answer
    {
        public Event EventPulled {get; private set;}

        internal Pull(FrontError err)
        :base(err)
        {
        }


        /// <summary>
        /// Sets field success to true and field eventPulled to evt.
        /// </summary>
        /// <param name="evt"></param>
        internal Pull(Event evt)
            :base()
        {
            EventPulled = evt;
        }
    }
}
