using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    abstract class Agent
    {

        protected UInt64 RequestID { get; private set; }

        protected Action<Agent> Callback;
        public String Message { get; protected set; }

        protected Agent(String Message, UInt64 RequestID, Action<Agent> Callback)
        {
            this.RequestID = RequestID;
            this.Callback = Callback;
        }

        public void Processed()
        {
            Callback(this);
        }

        public void ProcessFailed(String feedBackMessage)
        {
            // TODO
        }

    }
}
