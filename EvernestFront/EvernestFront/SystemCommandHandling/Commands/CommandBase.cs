using System;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{   //TODO: name of commands should end with Command
    abstract class CommandBase
    {
        private readonly SystemCommandHandler _systemCommandHandler;

        public readonly Guid Guid;

        protected CommandBase(SystemCommandHandler systemCommandHandler)
        {
            _systemCommandHandler = systemCommandHandler;
            Guid = Guid.NewGuid();
        }

        public void Send()
        {
            _systemCommandHandler.ReceiveCommand(this);
        }

        public abstract bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error);
    }
}
