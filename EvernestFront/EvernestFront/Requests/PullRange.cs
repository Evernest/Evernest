using System;

namespace EvernestFront.Requests

{
 
        class PullRange : Request
        {
 
            string eventIdFrom;
            string eventIdTo;
            /// <summary>
            /// Constructor for PullRange requests.
            /// Request pulls events between from and to (inclusive).
            /// </summary>
            /// <param name="user"></param>
            /// <param name="streamName"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            public PullRange(string user, string streamName, string from, string to)
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
                throw new NotImplementedException();
            }
        } 
    
}
