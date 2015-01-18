using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;


namespace EvernestBack
{
    class WriteLocker
	{
       private class PendingEvent
        {
            public string Message { get; private set; }
            public Action<IAgent> Callback { get; private set; }
            public PendingEvent(string message, Action<IAgent> callback)
            {
                Message = message;
                Callback = callback;
            }
        }

		private ConcurrentQueue<PendingEvent> PendingEventCollection = new ConcurrentQueue<PendingEvent>();
        private EventIndexer Indexer;
        private BufferedBlobIO WriteBuffer;
        public long CurrentId { get; private set; }

        public WriteLocker(BufferedBlobIO buffer, EventIndexer indexer, long firstId)
        {
            Indexer = indexer;
            WriteBuffer = buffer;
            CurrentId = firstId;
        }

        public void Store()
        {
            Task.Run(() =>
            {
                ulong wroteBytes;
                PendingEvent pendingEvent;
                while (true) //temporary fix to make sure the thread doesn't terminate early (well now it never does, "fixed")
                {
                    while (PendingEventCollection.TryDequeue(out pendingEvent))
                    {
                        Agent agent = new Agent(pendingEvent.Message, CurrentId, pendingEvent.Callback);
                        wroteBytes = Write(agent);
                        Indexer.NotifyNewEntry(CurrentId, wroteBytes);
                        CurrentId++;
                        agent.Processed();
                    }
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

        public void Register(string message, Action<IAgent> callback)
        {
            PendingEventCollection.Enqueue(new PendingEvent(message, callback));
        }
	}
}
