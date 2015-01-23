using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Projections;

namespace EvernestFront.SystemCommandHandling
{
    class Dispatcher
    {


        private ICollection<IProjection> Projections { set; get; }
        private SystemEventStream SystemEventStream { set; get; }

        private readonly ConcurrentQueue<Tuple<Guid,ISystemEvent>> _pendingEventQueue;
        private readonly EventWaitHandle _newTicket;

        private readonly CancellationTokenSource _tokenSource;

        private SystemCommandResultManager Manager { set; get; }

        public Dispatcher(ICollection<IProjection> projections, SystemEventStream systemEventStream, SystemCommandResultManager manager)
        {
            Projections = projections;
            SystemEventStream = systemEventStream;
            _pendingEventQueue = new ConcurrentQueue<Tuple<Guid, ISystemEvent>>();
            _tokenSource = new CancellationTokenSource();
            _newTicket = new AutoResetEvent(false);
            Manager = manager;
        }

        public void StopDispatching()
        {
            _tokenSource.Cancel();
        }

        public void ReceiveEvent(ISystemEvent systemEvent, Guid guid)
        {
            _pendingEventQueue.Enqueue(new Tuple<Guid, ISystemEvent>(guid, systemEvent));
            _newTicket.Set();
        }

        public void DispatchSystemEvents()
        {
            var token = _tokenSource.Token;
            Task.Run((() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Tuple<Guid, ISystemEvent> tuple;
                    while (_pendingEventQueue.TryDequeue(out tuple))
                        ConsumeSystemEvent(tuple.Item2, tuple.Item1);

                    _newTicket.WaitOne();
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
