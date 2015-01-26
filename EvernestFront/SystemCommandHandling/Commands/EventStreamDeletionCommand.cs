namespace EvernestFront.SystemCommandHandling.Commands
{
    class EventStreamDeletionCommand : CommandBase
    {
        internal long EventStreamId { get; private set; }
        internal string EventStreamName { get; private set; }
        internal string AdminName { get; private set; }
        internal long AdminId { get; private set; }
        internal string AdminPassword { get; private set; }

        internal EventStreamDeletionCommand(SystemCommandHandler systemCommandHandler, 
            long streamId, string streamName, string adminName, long adminId, string adminPassword)
            : base(systemCommandHandler)
        {
            AdminName = adminName;
            EventStreamId = streamId;
            EventStreamName = streamName;
            AdminId = adminId;
            AdminPassword = adminPassword;
        }
    }
}
