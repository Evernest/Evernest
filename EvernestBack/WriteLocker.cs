using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace EvernestBack
{
    internal class WriteLocker
    {
        private readonly EventIndexer _indexer;

        private readonly BlockingCollection<PendingEvent> _pendingEventCollection =
            new BlockingCollection<PendingEvent>();

        private readonly BufferedBlobIO _writeBuffer;

        public WriteLocker(BufferedBlobIO buffer, EventIndexer indexer, long firstID)
        {
            _indexer = indexer;
            _writeBuffer = buffer;
            CurrentID = firstID;
        }

        public long CurrentID { get; private set; }

        public void Store()
        {
            Task.Run(() =>
            {
                while (true)
                    //temporary fix to make sure the thread doesn't terminate early (well now it never does, "fixed")
                {
                    var pendingEvent = _pendingEventCollection.Take();
                    var agent = new Agent(pendingEvent.Message, CurrentID, pendingEvent.CallbackSuccess,
                        pendingEvent.CallbackFailure);
                    ulong wroteBytes = Write(agent);
                    _indexer.NotifyNewEntry(CurrentID, wroteBytes);
                    CurrentID++;
                    agent.Processed();
                }
            }
                );
        }

        private UInt16 Write(Agent prod)
        {
            UInt16 size;
            var bytes = prod.Serialize(out size);
            _writeBuffer.Push(bytes, 0, size);
            return size;
        }

        public void Register(string message, Action<IAgent> callbackSuccess, Action<IAgent, String> callBackFailure)
        {
            _pendingEventCollection.Add(new PendingEvent(message, callbackSuccess, callBackFailure));
        }

        private class PendingEvent
        {
            public PendingEvent(string message, Action<IAgent> callbackSuccess, Action<IAgent, String> callbackFailure)
            {
                Message = message;
                CallbackSuccess = callbackSuccess;
                CallbackFailure = callbackFailure;
            }

            public string Message { get; private set; }
            public Action<IAgent> CallbackSuccess { get; private set; }
            public Action<IAgent, String> CallbackFailure { get; private set; }
        }
    }
}