using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    class Producer:Agent
    {

        public Producer(String message, UInt64 requestID, EventStream feedback)
            :base(message, requestID, feedback)
        {
        }

        public static void Create(String message, UInt64 requestID, WriteLocker writeLocker, EventStream feedback)
        {
            Producer newProducer = new Producer(message, requestID, feedback);
            writeLocker.Register(newProducer);
        }

        public override void Processed()
        {
            this.Message = "ACK" + this.RequestID;
            base.feedback.StreamDeliver(this);
        }
    }
}
