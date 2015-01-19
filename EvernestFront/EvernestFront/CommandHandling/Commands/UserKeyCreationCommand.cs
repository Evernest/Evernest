using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.CommandHandling.Commands
{
    class UserKeyCreationCommand : CommandBase
    {
        internal readonly long UserId;

        internal readonly string KeyName;

        internal readonly string Key;

        internal UserKeyCreationCommand(CommandHandler commandHandler, long userId, string keyName, string key)
            :base(commandHandler)
        {
            UserId = userId;
            KeyName = keyName;
            Key = key;
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
            if (userData.KeyNames.Contains(KeyName))
            {
                error = FrontError.UserKeyNameTaken;
                systemEvent = null;
                return false;
            }
            systemEvent = new UserKeyCreatedSystemEvent(Key, UserId, KeyName);
            error = null;
            return true;

        }
    }
}
