using System;
using EvernestBack;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.CommandHandling.Commands
{
    class EventStreamCreationCommand : CommandBase
    {
        internal string EventStreamName { get; private set; }
        internal string CreatorName { get; private set; }
         
        internal EventStreamCreationCommand(CommandHandler commandHandler, string streamName, string creatorName)
            : base(commandHandler)
        {
            EventStreamName = streamName;
            CreatorName = creatorName;
        }

        public override bool TryToSystemEvent(CommandHandlingData serviceData, out ISystemEvent systemEvent, out FrontError? error)
        {
            if (serviceData.EventStreamNameExists(EventStreamName))
            {
                systemEvent = null;
                error = FrontError.EventStreamNameTaken;
                return false;
            }
            var id = serviceData.NextEventStreamId;
            AzureStorageClient.Instance.GetNewEventStream(Convert.ToString(id));
            systemEvent = new EventStreamCreatedSystemEvent(id, EventStreamName, CreatorName);
            error = null;
            return true;
        }
    }
}
