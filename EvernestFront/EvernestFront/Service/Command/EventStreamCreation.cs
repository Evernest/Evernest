namespace EvernestFront.Service.Command
{
    class EventStreamCreation : CommandBase
    {
        internal string EventStreamName { get; private set; }
        internal string CreatorName { get; private set; }
         
        internal EventStreamCreation(SystemEventProducer systemEventProducer, string streamName, string creatorName)
            : base(systemEventProducer)
        {
            EventStreamName = streamName;
            CreatorName = creatorName;
        }
    }
}
