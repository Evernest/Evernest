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

        private ConcurrentQueue<ISystemEvent> PendingEventQueue;

        private CancellationTokenSource tokenSource;

        public void StopDispatching()
        {
            tokenSource.Cancel();
        }

        public void HandleEvent(ISystemEvent systemEvent)
        {
            PendingEventQueue.Enqueue(systemEvent);
        }

        public void DispatchEvents()
        {
            var token = tokenSource.Token;
            Task.Run((() =>
            {
                while (!token.IsCancellationRequested)
                {
                    ISystemEvent systemEvent;
                    if (PendingEventQueue.TryDequeue(out systemEvent))
                        ConsumeSystemEvent(systemEvent);
                }
            }), token);
        }
    

        internal void ConsumeSystemEvent(ISystemEvent systemEvent)
        {

            SystemEventStream.Push(systemEvent);
            foreach (var p in Projections)
            {
                p.HandleSystemEvent(systemEvent);
            }
        }

        internal Dispatcher(ICollection<IProjection> projections, SystemEventStream systemEventStream)
        {
            Projections = projections;
            SystemEventStream = systemEventStream;
            PendingEventQueue = new ConcurrentQueue<ISystemEvent>();
            tokenSource=new CancellationTokenSource();
        }


    }
}
