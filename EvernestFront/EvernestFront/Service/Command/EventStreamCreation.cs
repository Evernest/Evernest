namespace EvernestFront.Service.Command
{
    class EventStreamCreation : CommandBase
    {
        internal string EventStreamName { get; private set; }
        internal string CreatorName { get; private set; }
         
        internal EventStreamCreation(string streamName, string creatorName)
        {
            EventStreamName = streamName;
            CreatorName = creatorName;
        }
    }
}
