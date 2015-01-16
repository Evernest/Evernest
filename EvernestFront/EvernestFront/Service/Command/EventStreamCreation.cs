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
    }
}
