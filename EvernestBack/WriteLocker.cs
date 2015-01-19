using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace EvernestBack
{
    internal class WriteLocker : IDisposable
    {
        private readonly EventIndexer _indexer;

        private readonly ConcurrentQueue<PendingEvent> _pendingEventCollection =
            new ConcurrentQueue<PendingEvent>();

        private readonly BufferedBlobIO _writeBuffer;
        private Thread _worker;
        private bool _keepOnWorking = true;
        EventWaitHandle _pushWaitHandle = new AutoResetEvent(false);

        public WriteLocker(BufferedBlobIO buffer, EventIndexer indexer, long firstId)
        {
            _indexer = indexer;
            _writeBuffer = buffer;
            _worker = new Thread(StoreLoop);
            _worker.Start();
            CurrentID = firstId;
        }

        public long CurrentID { get; private set; }

        private void StoreLoop()
        {
            PendingEvent pendingEvent;
            while (_keepOnWorking)
            {
                while (_pendingEventCollection.TryDequeue(out pendingEvent))
                {
                    var agent = new Agent(pendingEvent.Message, CurrentID, pendingEvent.CallbackSuccess,
                        pendingEvent.CallbackFailure);
                    int wroteBytes = Write(agent);
                    _indexer.NotifyNewEntry(CurrentID, (ulong)wroteBytes);
                    CurrentID++;
                    agent.Processed();
                }
                _pushWaitHandle.WaitOne();  
            }
        }

        private void StopWorker()
        {
            _keepOnWorking = false;
            _pushWaitHandle.Set();
            _worker.Join();
        }

        public void Dispose()
        {
            StopWorker();
        }

        private int Write(Agent prod)
        {
            int size;
            var bytes = prod.Serialize(out size);
            _writeBuffer.Push(bytes, 0, size);
            return size;
        }

        public void Register(string message, Action<IAgent> callbackSuccess, Action<IAgent, String> callBackFailure)
        {
            _pendingEventCollection.Enqueue(new PendingEvent(message, callbackSuccess, callBackFailure));
            _pushWaitHandle.Set();
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
