using System;

namespace EvernestFront.Requests
{
   
        class Push : Request
        {
            private Event eventToPush;

            public Push(string user, string streamName, Event eventToPush)
                : base(user, streamName)
            {
                this.eventToPush = eventToPush;
            }

            public override Answers.IAnswer Process()
            {
                throw new NotImplementedException();
            }
        } 
    
}
