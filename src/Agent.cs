using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    class Agent
    {
        protected Int64 requestID;

        protected Agent(Int64 requestID)
        {
            this.requestID = requestID;
        }
    }
}
