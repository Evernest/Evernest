using EvernestFront.Contract.SystemEvent;
using EvernestFront.Utilities;

namespace EvernestFront.Service.Command
{
    class UserKeyDeletion : CommandBase
    {
        internal string Key { get; set; } //base64 encoded int
        
        internal long UserId { get; set; }
        
        internal string KeyName { get; set; }

        internal UserKeyDeletion(CommandHandler commandHandler, string key, long userId, string keyName)
            : base(commandHandler)
        {
            Key = key;
            UserId = userId;
            KeyName = keyName;

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
            if (!userData.KeyNames.Contains(KeyName))
            {
                error = FrontError.UserKeyDoesNotExist;
                systemEvent = null;
                return false;
            }
            systemEvent= new UserKeyDeleted(Key, UserId, KeyName);
            error = null;
            return true;
        }
    }
}
