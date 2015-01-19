using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.SystemCommandHandling.Commands;

namespace EvernestFront.SystemCommandHandling
{
    class SystemCommandHandler
    {
        
        private readonly SystemCommandHandlerState _serviceData;
        private readonly Dispatcher _dispatcher;
        private readonly SystemCommandResultManager _manager;

        private readonly ConcurrentQueue<CommandBase> _pendingCommandQueue;
        private readonly CancellationTokenSource _tokenSource;
       

        public SystemCommandHandler(SystemCommandHandlerState serviceData, Dispatcher dispatcher, SystemCommandResultManager manager)
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

        public void StopHandlingCommands()
        {
            _tokenSource.Cancel();
        }

        public void HandleCommands()
        {
            var token = _tokenSource.Token;
            Task.Run((() =>
            {
                while (!token.IsCancellationRequested)
                {
                    CommandBase command;
                    if (_pendingCommandQueue.TryDequeue(out command))
                        HandleCommand(command);
                }
            }), token);
        }

        private void HandleCommand(CommandBase command)
        {
            ISystemEvent systemEvent;
            FrontError? error;
            if (command.TryToSystemEvent(_serviceData, out systemEvent, out error))
            {
                _serviceData.Update(systemEvent);
                _dispatcher.ReceiveEvent(systemEvent, command.Guid);
            }
            else
            {
                Debug.Assert(error != null, "error != null");
                _manager.AddCommandResult(command.Guid, new Response<Guid>((FrontError) error));
            }
        }
    }
}
