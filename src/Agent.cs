using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    abstract class Agent
    {
        protected Int64 requestID;

        protected Agent(Int64 requestID)
        {
            this.requestID = requestID;
        }

        public Int64 GetRequestID()
        {
            return requestID;
        }

    }
}
