using System;
using EvernestBack;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Service.Command
{
    class EventStreamCreation : CommandBase
    {
        internal string EventStreamName { get; private set; }
        internal string CreatorName { get; private set; }
         
        internal EventStreamCreation(CommandReceiver commandReceiver, string streamName, string creatorName)
            : base(commandReceiver)
        {
            EventStreamName = streamName;
            CreatorName = creatorName;
        }

        public override bool TryToSystemEvent(ServiceData serviceData, out Contract.SystemEvent.ISystemEvent systemEvent, out FrontError? error)
        {
            if (serviceData.EventStreamNameExists(EventStreamName))
            {
                systemEvent = null;
                error = FrontError.EventStreamNameTaken;
                return false;
            }
            var id = serviceData.NextEventStreamId;
            AzureStorageClient.Instance.GetNewEventStream(Convert.ToString(id));
            systemEvent = new EventStreamCreated(id, EventStreamName, CreatorName);
            error = null;
            return true;
        }
    }
}
