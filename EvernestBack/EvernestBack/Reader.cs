using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    class Reader:Agent
    {
        public Reader(String message, UInt64 requestID, EventStream feedback)
            :base(message, requestID, feedback)
        {
            
        }
        
        private void read()
        {
            //TODO
            // message = "Aller chercher le message sur le blob"
        }

        public override void Processed()
        {
            base.feedback.StreamDeliver(this);
        }
    }
}
