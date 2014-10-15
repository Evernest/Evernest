﻿using System;
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
                try
                {
                    Stream stream = StreamTable.GetStream(StreamName);
                    RightsTable.CheckCanWrite(User, StreamName);
                    return stream.Push(eventToPush);
                }
                catch (StreamNameDoesNotExistException exn)
                {
                    return new Answers.Push(exn);
                }
                catch (AccessDeniedException exn)
                {
                    return new Answers.Push(exn);
                }
            }
        } 
    
}
