using System;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests
{
   
        class PullRandom : Request
        {   
            /// <summary>
            /// Constructor for PullRandom requests.
            /// </summary>
            /// <param name="user"></param>
            /// <param name="streamName"></param>
            public PullRandom(string user, string streamName)
                : base(user, streamName) { }

            /// <summary>
            /// Processes PullRandom request with a backend call. Request is successful if user has reading rights.
            /// </summary>
            /// <returns></returns>
            public override Answers.PullRandom Process()
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
