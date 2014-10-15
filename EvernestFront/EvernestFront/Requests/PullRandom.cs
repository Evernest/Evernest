using System;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests
{
   
        class PullRandom : Request<Answers.PullRandom>
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
                    RightsTable.CheckCanRead(User,StreamName);
                    return stream.PullRandom();
                }
                catch (StreamNameDoesNotExistException exn)
                {
                    return new Answers.PullRandom(exn);
                }
                catch (AccessDeniedException exn)
                {
                    return new Answers.PullRandom(exn);
                }
                
            }
        }
    
}
