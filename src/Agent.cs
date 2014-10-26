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
        protected String message;

        protected Agent(String Message, Int64 requestID, Stream feedback)
        {
            this.requestID = requestID;
            this.feedback = feedback;
        }

        public Int64 GetRequestID()
        {
            return requestID;
        }

        public String GetMessage()
        {
            return message;
        }

    }
}
