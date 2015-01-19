using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class SourceCreationCommand : CommandBase
    {
        internal readonly long UserId;

        internal readonly string SourceName;

        internal readonly string SourceKey;

        internal SourceCreationCommand(SystemCommandHandler systemCommandHandler, long userId, string sourceName, string sourceKey)
            :base(systemCommandHandler)
        {
            UserId = userId;
            SourceName = sourceName;
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
            if (userRecord.SourceNames.Contains(SourceName))
            {
                error = FrontError.SourceNameTaken;
                systemEvent = null;
                return false;
            }
            systemEvent = new SourceCreatedSystemEvent(SourceKey, SourceName, userRecord.NextSourceId, UserId);
            error = null;
            return true;
        }
    }
}
