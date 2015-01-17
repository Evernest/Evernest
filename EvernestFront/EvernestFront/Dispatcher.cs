using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Projections;

namespace EvernestFront
{
    class Dispatcher
    {


        private ICollection<IProjection> Projections { set; get; }
        private SystemEventStream SystemEventStream { set; get; }

        private readonly ConcurrentQueue<ISystemEvent> _pendingEventQueue;

        private readonly CancellationTokenSource _tokenSource;

        public Dispatcher(ICollection<IProjection> projections, SystemEventStream systemEventStream)
        {
            Projections = projections;
            SystemEventStream = systemEventStream;
            _pendingEventQueue = new ConcurrentQueue<ISystemEvent>();
            _tokenSource = new CancellationTokenSource();
        }

        public void StopDispatching()
        {
            _tokenSource.Cancel();
        }

        public void HandleEvent(ISystemEvent systemEvent)
        {
            _pendingEventQueue.Enqueue(systemEvent);
        }

        public void DispatchEvents()
        {
            var token = _tokenSource.Token;
            Task.Run((() =>
            {
                while (!token.IsCancellationRequested)
                {
                    ISystemEvent systemEvent;
                    if (_pendingEventQueue.TryDequeue(out systemEvent))
                        ConsumeSystemEvent(systemEvent);
                }
            }), token);
        }
    

        internal void ConsumeSystemEvent(ISystemEvent systemEvent)
        {

            SystemEventStream.Push(systemEvent);
            foreach (var p in Projections)
            {
                p.OnSystemEvent(systemEvent);
            }
        }

       


    }
}
