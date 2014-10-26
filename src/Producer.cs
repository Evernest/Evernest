using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    class Producer:Agent
    {
        private Message message;
        private WriteLocker writer;

        public Producer(Message message, Int64 requestId, 
            WriteLocker writeLocker, Stream feedback)
            :base(requestId, feedback)
        {
            this.message = message;
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
            this.message = new Message("ACK", this.message.getID());
            base.feedback.StreamDeliver(this);
        }

        public void ProcessFailed(String feedBackMessage)
        {
        }
    }
}
