namespace EvernestFront.SystemCommandHandling.Commands
{
    class SourceDeletionCommand : CommandBase
    {
        internal readonly long UserId;

        internal readonly long SourceId;

        internal readonly string SourceKey;

        internal SourceDeletionCommand(SystemCommandHandler systemCommandHandler, long userId, long sourceId, string sourceKey)
            :base(systemCommandHandler)
        {
            UserId = userId;
            SourceId = sourceId;
            SourceKey = sourceKey;
        }
    }
}
