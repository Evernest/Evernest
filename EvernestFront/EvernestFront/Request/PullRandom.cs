using System;

namespace EvernestFront.Request
{
   
        class PullRandom : Request
        {
            public PullRandom(string user, string streamName)
                : base(user, streamName) { }

            public override IAnswer Process()
            {
                try
                {
                    Stream stream = StreamTable.GetStream(StreamName);
                    return stream.PullRandom(User);
                }
                catch (StreamNameDoesNotExistException e)
                {
                    throw new NotImplementedException();
                }
                
            }
        }
    
}
