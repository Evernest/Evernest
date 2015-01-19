using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

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

        public override bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!systemCommandHandlerState.UserIdToData.TryGetValue(UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            string sourceName;
            if (!userRecord.SourceIdToName.TryGetValue(SourceId, out sourceName))
            {
                error = FrontError.SourceIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            systemEvent = new SourceDeletedSystemEvent(SourceKey, sourceName, SourceId, UserId);
            error = null;
            return true;
        }
    }
}
