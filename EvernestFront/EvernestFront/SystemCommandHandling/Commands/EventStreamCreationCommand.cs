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
         
        internal EventStreamCreationCommand(SystemCommandHandler systemCommandHandler, string streamName, string creatorName)
            : base(systemCommandHandler)
        {
            EventStreamName = streamName;
            CreatorName = creatorName;
        }

        public override bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error)
        {
            if (!systemCommandHandlerState.UserNames.Contains(CreatorName))
            {
                systemEvent = null;
                error = FrontError.UserNameDoesNotExist;
                return false;
            }
            if (systemCommandHandlerState.EventStreamNames.Contains(EventStreamName))
            {
                systemEvent = null;
                error = FrontError.EventStreamNameTaken;
                return false;
            }
            var id = systemCommandHandlerState.NextEventStreamId;
            AzureStorageClient.Instance.GetNewEventStream(Convert.ToString(id));
            systemEvent = new EventStreamCreatedSystemEvent(id, EventStreamName, CreatorName);
            error = null;
            return true;
        }
    }
}
