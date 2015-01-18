using System;
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

        private readonly ConcurrentQueue<Tuple<Guid,ISystemEvent>> _pendingEventQueue;

        private readonly CancellationTokenSource _tokenSource;

        private CommandResultManager Manager { set; get; }

        public Dispatcher(ICollection<IProjection> projections, SystemEventStream systemEventStream, CommandResultManager manager)
        {
            Projections = projections;
            SystemEventStream = systemEventStream;
            _pendingEventQueue = new ConcurrentQueue<Tuple<Guid, ISystemEvent>>();
            _tokenSource = new CancellationTokenSource();
            Manager = manager;
        }

        public void StopDispatching()
        {
            _tokenSource.Cancel();
        }

        public void ReceiveEvent(ISystemEvent systemEvent, Guid guid)
        {
            _pendingEventQueue.Enqueue(new Tuple<Guid, ISystemEvent>(guid, systemEvent));
        }

        public void DispatchSystemEvents()
        {
            var token = _tokenSource.Token;
            Task.Run((() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Tuple<Guid, ISystemEvent> tuple;
                    if (_pendingEventQueue.TryDequeue(out tuple))
                        ConsumeSystemEvent(tuple.Item2, tuple.Item1);
                }
            }), token);
        }
    

        private void ConsumeSystemEvent(ISystemEvent systemEvent, Guid guid)
        {

            SystemEventStream.Push(systemEvent);
            foreach (var p in Projections)
            {
                p.OnSystemEvent(systemEvent);
            }
            Manager.AddCommandResult(guid, new Response<Guid>(guid));
        }

       


    }
}
