using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Service.Command
{
    class UserKeyCreation : CommandBase
    {
        internal readonly long UserId;

        internal readonly string KeyName;

        internal readonly string Key;

        internal UserKeyCreation(CommandHandler commandHandler, long userId, string keyName, string key)
            :base(commandHandler)
        {
            UserId = userId;
            KeyName = keyName;
            Key = key;
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
            if (userData.KeyNames.Contains(KeyName))
            {
                error = FrontError.UserKeyNameTaken;
                systemEvent = null;
                return false;
            }
            systemEvent = new UserKeyCreated(Key, UserId, KeyName);
            error = null;
            return true;

        }
    }
}
