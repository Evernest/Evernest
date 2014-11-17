using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    class Producer:Agent
    {
        private WriteLocker writer;

        public Producer(String message, UInt64 requestId, 
            WriteLocker writeLocker, EventStream feedback)
            :base(message, requestId, feedback)
        {
            this.writer = writeLocker;
            write();
        }
        
        private void write()
        {
            writer.Register(this);
        }

        public override void Processed()
        {
            this.Message = "ACK" + this.RequestID;
            base.feedback.StreamDeliver(this);
        }
    }
}
