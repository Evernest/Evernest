using System;
using EvernestFront.Exceptions;


namespace EvernestFront.Requests
{
   
        class Push : Request
        {
            private Event eventToPush;
            /// <summary>
            /// Constructor for Push requests.
            /// </summary>
            /// <param name="user"></param>
            /// <param name="streamName"></param>
            /// <param name="eventToPush"></param>
            public Push(string user, string streamName, Event eventToPush)
                : base(user, streamName)
            {
                this.eventToPush = eventToPush;
            }

            /// <summary>
            /// Processes Push request. Request is successful if user has writing rights.
            /// </summary>
            /// <returns></returns>
            public override Answers.Push Process()
            {
                throw new NotImplementedException();
            }
        } 
    
}
