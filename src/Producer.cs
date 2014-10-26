using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    class Producer:Agent
    {
        private WriteLocker writer;

        public Producer(String message, Int64 requestId, 
            WriteLocker writeLocker, Stream feedback)
            :base(message, requestId, feedback)
        {
            this.writer = writeLocker;
            write();
        }

        //public Message GetMessage()
        //{
        //    return message;
        //}
        
        private void write()
        {
            writer.Register(this);
        }

        public void Processed()
        {
            this.message = "ACK" + this.GetRequestID();
            base.feedback.StreamDeliver(this);
        }

        public void ProcessFailed(String feedBackMessage)
        {
        }
    }
}
