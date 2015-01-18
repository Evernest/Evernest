using System;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.CommandHandling.Commands
{   //TODO: name of commands should end with Command
    abstract class CommandBase
    {
        private readonly CommandHandler _commandHandler;

        public readonly Guid Guid;

        protected CommandBase(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
            Guid = Guid.NewGuid();
        }

        public void Send()
        {
            _commandHandler.ReceiveCommand(this);
        }

        public abstract bool TryToSystemEvent(ServiceData serviceData, out ISystemEvent systemEvent, out FrontError? error);
    }
}
