using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    abstract class Agent
    {
        protected Int64 requestID { get; private set; }
        protected Stream feedback;
        public String message { get; protected set; }

        protected Agent(String Message, Int64 requestID, Stream feedback)
        {
            this.requestID = requestID;
            this.feedback = feedback;
        }

        abstract public void Processed();

        public void ProcessFailed(String feedBackMessage)
        {
        }
    }
}
