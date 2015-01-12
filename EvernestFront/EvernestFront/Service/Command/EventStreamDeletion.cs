namespace EvernestFront.Service.Command
{
    class EventStreamDeletion : CommandBase
    {
        internal long EventStreamId { get; private set; }
        internal string EventStreamName { get; private set; }
        internal long AdminId { get; private set; }
        internal string AdminPassword { get; private set; }

        internal EventStreamDeletion(long streamId, string streamName, long adminId, string adminPassword)
        {
            EventStreamId = streamId;
            EventStreamName = streamName;
            AdminId = adminId;
            AdminPassword = adminPassword;
        }
    }
}
