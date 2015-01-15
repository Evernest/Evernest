using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public CommandReceiver(SystemEventProducer systemEventProducer,
            ServiceData serviceData, Dispatcher dispatcher)
        {
            _systemEventProducer = systemEventProducer;
            _serviceData = serviceData;
            _dispatcher = dispatcher;
            KeepRunning = true;
        }

        private ConcurrentQueue<CommandBase> _pendingCommandQueue = new ConcurrentQueue<CommandBase>();

        public void HandleCommand(CommandBase command)
        {
            _pendingCommandQueue.Enqueue(command);
        }

        public bool KeepRunning; //change to false to stop producing events

        private void ProduceEvents()
        {
            Task.Run(() =>
            {
                while (KeepRunning)
                {
                    CommandBase command;
                    if (_pendingCommandQueue.TryDequeue(out command))
                        ConsumeCommand(command);
                }
            });
        }

        private void ConsumeCommand(CommandBase command)
        {
            ISystemEvent systemEvent = _systemEventProducer.CommandToSystemEvent(command);
            _serviceData.SelfUpdate(systemEvent);
            _dispatcher.HandleSystemEventEnvelope(systemEvent);
        }
    }
}
