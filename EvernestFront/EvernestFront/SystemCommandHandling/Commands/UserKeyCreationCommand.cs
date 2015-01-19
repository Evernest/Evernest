using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class UserKeyCreationCommand : CommandBase
    {
        internal readonly long UserId;

        internal readonly string KeyName;

        internal readonly string Key;

        internal UserKeyCreationCommand(SystemCommandHandler systemCommandHandler, long userId, string keyName, string key)
            :base(systemCommandHandler)
        {
            UserId = userId;
            KeyName = keyName;
            Key = key;
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
            if (userRecord.KeyNames.Contains(KeyName))
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
