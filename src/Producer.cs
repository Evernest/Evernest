using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud14
{
    class Producer
    {
        private Message message;
        private Int64 requestID;
        public Producer(Message message, Int64 requestId)
        {
            this.message = message;
            this.requestID = requestId;
        }

        public void RequestWrite()
        {
        }
        
        /**
         * Cette méthode est appellée lorsque le Producer à l'autorisation d'écrire.
         */
        private void write()
        {
            Console.Write("Méthode Producer.Write() sur le thread {0}",
                Thread.CurrentThread.ManagedThreadId);
        }
    }
}
