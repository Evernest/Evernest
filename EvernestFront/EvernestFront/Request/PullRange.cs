using System;

namespace EvernestFront.Request
{
 
        class PullRange : Request
        {
            
            string eventIdFrom;
            string eventIdTo;

            public PullRange(string user, string streamName, string from, string to)
                : base(user, streamName)
            {

                this.eventIdFrom = from;
                this.eventIdTo = to;
            }

            public override IAnswer Process()
            {
                throw new NotImplementedException();
            }
        } 
    
}
