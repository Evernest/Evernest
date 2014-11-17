using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    abstract class Agent
    {
        protected UInt64 requestID { get; private set; }
        protected EventStream feedback;
        public String message { get; protected set; }

        protected Agent(String Message, UInt64 requestID, EventStream feedback)
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
