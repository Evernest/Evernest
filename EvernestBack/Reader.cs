using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    class Reader:Agent
    {
        public Reader(String Message, UInt64 RequestID, Action<IAgent> Callback)
            :base(Message, RequestID, Callback)
        {
            
        }
        
        private void Read()
        {
            //TODO
            // message = "Pull from blob"
        }
    }
}
