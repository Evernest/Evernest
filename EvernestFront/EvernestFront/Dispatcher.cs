using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Projections;
using EvernestFront.Service;

namespace EvernestFront
{
    class Dispatcher
    {


        private ICollection<IProjection> Projections { set; get; }
        private SystemEventStream SystemEventStream { set; get; }

        private ConcurrentQueue<ISystemEvent> _pendingEventQueue;

        private CancellationTokenSource _tokenSource;

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
