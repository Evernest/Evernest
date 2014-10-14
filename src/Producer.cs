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

        public Producer(Message message, Int64 requestId):base(requestId)
        {
            this.message = message;
        }

        public Message GetMessage()
        {
            return message;
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
