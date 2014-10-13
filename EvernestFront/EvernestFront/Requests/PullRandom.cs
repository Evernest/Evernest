using System;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests
{
   
        class PullRandom : Request
        {
            public PullRandom(string user, string streamName)
                : base(user, streamName) { }

            public override Answers.IAnswer Process()
            {
                try
                {
                    Stream stream = StreamTable.GetStream(StreamName);
                    return stream.PullRandom(User);
                }
                catch (StreamNameDoesNotExistException exn)
                {
                    return new Answers.PullRandom(exn);
                }
                
            }
        }
    
}
