using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.CommandHandling.Commands
{
    class SourceCreationCommand : CommandBase
    {
        internal readonly long UserId;

        internal readonly string SourceName;

        internal readonly string SourceKey;

        internal SourceCreationCommand(CommandHandler commandHandler, long userId, string sourceName, string sourceKey)
            :base(commandHandler)
        {
            UserId = userId;
            SourceName = sourceName;
            SourceKey = sourceKey;
        }

        public override bool TryToSystemEvent(CommandHandlingData commandHandlingData, out ISystemEvent systemEvent, out FrontError? error)
        {
            CommandHandlingUserData userData;
            if (!commandHandlingData.UserIdToData.TryGetValue(UserId, out userData))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (userData.SourceNames.Contains(SourceName))
            {
                error = FrontError.SourceNameTaken;
                systemEvent = null;
                return false;
            }
            systemEvent = new SourceCreatedSystemEvent(SourceKey, SourceName, userData.NextSourceId, UserId);
            error = null;
            return true;
        }
    }
}
