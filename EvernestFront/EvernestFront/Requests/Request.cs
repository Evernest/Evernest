using System;

namespace EvernestFront.Requests
{
  
        abstract class Request<TAnswer> where TAnswer : Answers.Answer
        {
            protected string User;

            protected string StreamName;

            public override string ToString()
            {
                throw new NotImplementedException();
            }

            protected Request(string user, string streamName)
            {
                this.User = user;
                this.StreamName = streamName;
            }

            /// <summary>
            /// Processes the request with a back-end call.
            /// </summary>
            /// <returns></returns>
            public abstract TAnswer Process();
           
            
        } 
    
}
