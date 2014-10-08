using System;

namespace EvernestFront.Request
{
   
        class PullRandom : Request
        {
            public PullRandom(string user, string streamName)
                : base(user, streamName) { }

            public override IAnswer Process()
            {
                throw new NotImplementedException();
            }
        }
    
}
