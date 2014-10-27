using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    class Reader:Agent
    {
        public Reader(String message, Int64 requestID, Stream feedback)
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
