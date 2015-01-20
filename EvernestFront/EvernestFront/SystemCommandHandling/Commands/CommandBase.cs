using System;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
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
    }
}
