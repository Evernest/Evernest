using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class UserKeyDeletionCommand : CommandBase
    {
        internal string Key { get; set; } //base64 encoded int
        
        internal long UserId { get; set; }
        
        internal string KeyName { get; set; }

        internal UserKeyDeletionCommand(SystemCommandHandler systemCommandHandler, string key, long userId, string keyName)
            : base(systemCommandHandler)
        {
            Key = key;
            UserId = userId;
            KeyName = keyName;
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
            if (!userRecord.KeyNames.Contains(KeyName))
            {
                error = FrontError.UserKeyDoesNotExist;
                systemEvent = null;
                return false;
            }
            systemEvent= new UserKeyDeletedSystemEvent(Key, UserId, KeyName);
            error = null;
            return true;
        }
    }
}
