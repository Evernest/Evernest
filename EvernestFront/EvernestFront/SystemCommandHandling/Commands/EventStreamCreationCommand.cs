using System;
using EvernestBack;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class EventStreamCreationCommand : CommandBase
    {
        internal string EventStreamName { get; private set; }
        internal string CreatorName { get; private set; }
        internal long CreatorId { get; private set; }
         
        internal EventStreamCreationCommand(SystemCommandHandler systemCommandHandler, string streamName, string creatorName, long creatorId)
            : base(systemCommandHandler)
        {
            CreatorId = creatorId;
            EventStreamName = streamName;
            CreatorName = creatorName;
        }
    }
}
