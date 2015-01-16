using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Service.Command;

namespace EvernestFront.Service
{
    class CommandReceiver
    {
        
        private SystemEventProducer _systemEventProducer;
        private ServiceData _serviceData;
        private Dispatcher _dispatcher;

        private ConcurrentQueue<CommandBase> _pendingCommandQueue;
        private CancellationTokenSource _tokenSource;

        public CommandReceiver(SystemEventProducer systemEventProducer,
            ServiceData serviceData, Dispatcher dispatcher)
        {
            _systemEventProducer = systemEventProducer;
            _serviceData = serviceData;
            _dispatcher = dispatcher;
            _pendingCommandQueue=new ConcurrentQueue<CommandBase>();
            _tokenSource=new CancellationTokenSource();
        }
        

        public void ReceiveCommand(CommandBase command)
        {
            _pendingCommandQueue.Enqueue(command);
        }

        public void StopProducing()
        {
            _tokenSource.Cancel();
        }

        public void ProduceEvents()
        {
            var token = _tokenSource.Token;
            Task.Run((() =>
            {
                while (!token.IsCancellationRequested)
                {
                    CommandBase command;
                    if (_pendingCommandQueue.TryDequeue(out command))
                        ConsumeCommand(command);
                }
            }), token);
        }

        private void ConsumeCommand(CommandBase command)
        {
            ISystemEvent systemEvent = _systemEventProducer.CommandToSystemEvent(command);
            _serviceData.SelfUpdate(systemEvent);
            _dispatcher.ConsumeSystemEvent(systemEvent);
        }
    }
}
