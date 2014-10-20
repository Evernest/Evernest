using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    abstract class Agent
    {
        protected Int64 requestID;
        protected Stream feedback;
        private Message message;

        protected Agent(Int64 requestID, Stream feedback)
        {
            this.requestID = requestID;
            this.feedback = feedback;
        }

        public Int64 GetRequestID()
        {
            return requestID;
        }

        public Message GetMessage()
        {
            return message;
        }

    }
}
