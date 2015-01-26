using System;

namespace EvernestFront.SystemCommandHandling.Commands
{
    /// <summary>
    /// Represents an administration command (eg. stream/user/source creation or deletion, changing user rights...) that has been accepted by the projections
    /// (which means it will be successful except if very recent commands invalidate it).
    /// These commands have a GUID that can be used by client apps to track their status.
    /// </summary>
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
