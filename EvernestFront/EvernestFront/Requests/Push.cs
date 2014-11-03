using System;
using EvernestFront.Exceptions;


namespace EvernestFront.Requests
{
   
        class Push : Request<Answers.Push>
        {
            private Event eventToPush;
            /// <summary>
            /// Constructor for Push requests.
            /// </summary>
            /// <param name="user"></param>
            /// <param name="streamName"></param>
            /// <param name="eventToPush"></param>
            internal Push(string user, string streamName, Event eventToPush)
                : base(user, streamName)
            {
                this.eventToPush = eventToPush;
            }

            /// <summary>
            /// Processes Push request. Request is successful if user has writing rights.
            /// </summary>
            /// <returns></returns>
            internal override Answers.Push Process()
            {
                try
                {
                    CheckRights.CheckCanWrite(User, StreamName);
                    Stream stream = StreamTable.GetStream(StreamName);
                    return stream.Push(eventToPush);
                }
                catch (FrontException exn)
                {
                    return new Answers.Push(exn);
                }

            }
        } 
    
}
