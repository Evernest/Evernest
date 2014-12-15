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
        private LocalCache Cache;
        private BufferedBlobIO WriteBuffer;
        public UInt64 CurrentID {get ; private set;}
        
        public WriteLocker(BufferedBlobIO buffer, LocalCache cache, UInt64 firstID)
        {
            Cache = cache;
            WriteBuffer = buffer;
            CurrentID = firstID;

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
                    Cache.NotifyNewEntry(CurrentID, wroteBytes);
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
