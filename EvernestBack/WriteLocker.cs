using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;


namespace EvernestBack
{
    class WriteLocker
	{
       private class PendingEvent
        {
            public string Message { get; private set; }
            public Action<IAgent> CallbackSuccess { get; private set; }
            public Action<IAgent, String> CallbackFailure { get; private set; }
            public PendingEvent(string message, Action<IAgent> callbackSuccess, Action<IAgent, String> callbackFailure)
            {
                Message = message;
                CallbackSuccess = callbackSuccess;
                CallbackFailure = callbackFailure;
            }
        }

		private BlockingCollection<PendingEvent> PendingEventCollection = new BlockingCollection<PendingEvent>();
        private EventIndexer Indexer;
        private BufferedBlobIO WriteBuffer;
        public long CurrentID {get ; private set;}
       
        public WriteLocker(BufferedBlobIO buffer, EventIndexer indexer, long firstID)
        {
            Indexer = indexer;
            WriteBuffer = buffer;
            CurrentID = firstID;
        }

        public void Store()
        {
            Task.Run(() =>
            {
                long wroteBytes;
                //Console.WriteLine("Starting Storing"); //if there is a Console.Read() in the main thread, this will block this instruction
                while (PendingEventCollection.Count > 0)
                {
                    PendingEvent pendingEvent = PendingEventCollection.Take();
                    Agent agent = new Agent(pendingEvent.Message, CurrentID, pendingEvent.CallbackSuccess, pendingEvent.CallbackFailure);
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

        public void Register(string message, Action<IAgent> callbackSuccess, Action<IAgent, String> callBackFailure)
        {
            PendingEventCollection.Add(new PendingEvent(message, callbackSuccess, callBackFailure));
        }
	}
}
