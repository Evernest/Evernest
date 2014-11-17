using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    public abstract class Agent:IAgent
    {
        private Action<IAgent> Callback;


        public UInt64 RequestID {get; private set; }
        public String Message {get; protected set; }

        internal Agent(String Message, UInt64 RequestID, Action<IAgent> Callback)
        {
            this.RequestID = RequestID;
            this.Callback = Callback;
        }

        internal void Processed()
        {
            Callback(this);
        }

        internal void ProcessFailed(String feedBackMessage)
        {
            // TODO
        }

    }
}
