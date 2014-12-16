using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;


namespace EvernestBack
{
    class WriteLocker
	{
       private class PendingEvent
        {
            public String Message { get; private set; }
            public Action<IAgent> Callback { get; private set; }
            public PendingEvent(String message, Action<IAgent> callback)
            {
                Message = message;
                Callback = callback;
            }
        }
		private BlockingCollection<PendingEvent> PendingEventCollection = new BlockingCollection<PendingEvent>();
        private EventIndexer Indexer;
        private BufferedBlobIO WriteBuffer;
        private UInt64 CurrentID;
        
        public WriteLocker(BufferedBlobIO buffer, EventIndexer indexer)
        {
            Indexer = indexer;
            WriteBuffer = buffer;
            CurrentID = 0;
        }

        public void Store()
        {
            Task.Run(() =>
            {
                UInt16 wroteBytes;
                //Console.WriteLine("Starting Storing"); //if there is a Console.Read() in the main thread, this will block this instruction
                while (PendingEventCollection.Count > 0)
                {
                    PendingEvent pendingEvent = PendingEventCollection.Take();
                    Agent agent = new Agent(pendingEvent.Message, CurrentID, pendingEvent.Callback);
                    wroteBytes = Write(agent);
                    Indexer.NotifyNewEntry(CurrentID, wroteBytes);
                    CurrentID++;
                    agent.Processed();
                }
            }
            );
        }

        private UInt16 Write(Agent prod)
        {
            UInt16 size;
            Byte[] bytes = prod.Serialize(out size);
            WriteBuffer.Push(bytes, 0, size);
            return size;
        }

        public void Register(String message, Action<IAgent> callback)
        {
            PendingEventCollection.Add(new PendingEvent(message, callback));
        }
	}
}
