using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    abstract class Agent
    {

        protected UInt64 requestID { get; private set; }

        protected Action<Agent> callback;
        public String message { get; protected set; }

        protected Agent(String Message, UInt64 requestID, Action<Agent> Callback)
        {
            this.requestID = requestID;
            this.callback = Callback;
        }

        abstract public void Processed();

        public void ProcessFailed(String feedBackMessage)
        {
        }

    }
}
