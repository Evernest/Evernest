using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Responses;
using EvernestFront.Service.Command;

namespace EvernestFront.Service
{
    class CommandReceiver
    {
        
        private readonly ServiceData _serviceData;
        private readonly Dispatcher _dispatcher;
        private readonly CommandResultManager _manager;

        private readonly ConcurrentQueue<CommandBase> _pendingCommandQueue;
        private readonly CancellationTokenSource _tokenSource;
       

        public CommandReceiver(ServiceData serviceData, Dispatcher dispatcher, CommandResultManager manager)
        {
            _serviceData = serviceData;
            _dispatcher = dispatcher;
            _manager = manager;
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
            ISystemEvent systemEvent;
            FrontError? error;
            if (command.TryToSystemEvent(_serviceData, out systemEvent, out error))
            {
                _serviceData.SelfUpdate(systemEvent);
                _dispatcher.HandleEvent(systemEvent, command.Guid);
            }
            else
            {
                Debug.Assert(error != null, "error != null");
                _manager.AddCommandResult(command.Guid, new SystemCommandResponse((FrontError) error));
            }
        }
    }
}
