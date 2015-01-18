using EvernestFront.Contract.SystemEvent;
using EvernestFront.Utilities;

namespace EvernestFront.Service.Command
{
    class UserKeyCreation : CommandBase
    {
        internal long UserId { get; set; }
        
        internal string KeyName { get; set; }

        internal UserKeyCreation(CommandHandler commandHandler, long userId, string keyName)
            :base(commandHandler)
        {
            UserId = userId;
            KeyName = keyName;
        }

        public override bool TryToSystemEvent(ServiceData serviceData, out Contract.SystemEvent.ISystemEvent systemEvent, out FrontError? error)
        {
            UserDataForService userData;
            if (!serviceData.UserIdToDatas.TryGetValue(UserId, out userData))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (userData.Keys.Contains(KeyName))
            {
                error = FrontError.UserKeyNameTaken;
                systemEvent = null;
                return false;
            }
            var keyGenerator = new KeyGenerator();
            var key = keyGenerator.NewKey();
            systemEvent = new UserKeyCreated(key, UserId, KeyName);
            error = null;
            return true;

        }
    }
}
