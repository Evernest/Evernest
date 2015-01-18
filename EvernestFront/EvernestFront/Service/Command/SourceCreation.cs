using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Service.Command
{
    class SourceCreation : CommandBase
    {
        internal readonly long UserId;

        internal readonly string SourceName;

        internal readonly long SourceId;

        internal readonly string SourceKey;

        internal SourceCreation(CommandHandler commandHandler, long userId, string sourceName, long sourceId, string sourceKey)
            :base(commandHandler)
        {
            UserId = userId;
            SourceName = sourceName;
            SourceId = sourceId;
            SourceKey = sourceKey;
        }

        public override bool TryToSystemEvent(ServiceData serviceData, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserDataForService userData;
            if (!serviceData.UserIdToDatas.TryGetValue(UserId, out userData))
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
            systemEvent = new SourceCreated(SourceKey, SourceName, SourceId, UserId);
            error = null;
            return true;
        }
    }
}
