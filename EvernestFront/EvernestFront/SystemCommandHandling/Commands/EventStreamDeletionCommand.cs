using System.Collections.Generic;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class EventStreamDeletionCommand : CommandBase
    {
        internal long EventStreamId { get; private set; }
        internal string EventStreamName { get; private set; }
        internal long AdminId { get; private set; }
        internal string AdminPassword { get; private set; }

        internal EventStreamDeletionCommand(SystemCommandHandler systemCommandHandler, 
            long streamId, string streamName, long adminId, string adminPassword)
            : base(systemCommandHandler)
        {
            EventStreamId = streamId;
            EventStreamName = streamName;
            AdminId = adminId;
            AdminPassword = adminPassword;
        }
    }
}
