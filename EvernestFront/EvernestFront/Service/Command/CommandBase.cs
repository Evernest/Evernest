using System;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Service.Command
{
    abstract class CommandBase
    {
        //TODO: add an id

        private readonly CommandReceiver _commandReceiver;

        public readonly Guid Guid;

        protected CommandBase(CommandReceiver commandReceiver)
        {
            _commandReceiver = commandReceiver;
            Guid = Guid.NewGuid();
        }

        public void Send()
        {
            _commandReceiver.ReceiveCommand(this);
        }

        public abstract bool TryToSystemEvent(ServiceData serviceData, out ISystemEvent systemEvent, out FrontError? error);
    }
}
