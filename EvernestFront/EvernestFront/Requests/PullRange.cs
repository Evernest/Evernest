using System;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests

{
 
        class PullRange : Request<Answers.PullRange>
        {
 
            int eventIdFrom;
            int eventIdTo;
            /// <summary>
            /// Constructor for PullRange requests.
            /// Request pulls events between from and to (inclusive).
            /// </summary>
            /// <param name="user"></param>
            /// <param name="streamName"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            internal PullRange(string user, string streamName, int from, int to)
                : base(user, streamName)
            {

                this.eventIdFrom = from;
                this.eventIdTo = to;
            }
            /// <summary>
            /// Processes PullRange request. Request is successful if user has reading rights.
            /// </summary>
            /// <returns></returns>
            public override Answers.PullRange Process()
            {
                try
                {
                    CheckRights.CheckCanRead(User, StreamName);
                    Stream stream = StreamTable.GetStream(StreamName);
                    return stream.PullRange(eventIdFrom,eventIdTo);
                }
                catch (FrontException exn)
                {
                    return new Answers.PullRange(exn);
                }

            }
        } 
    
}
