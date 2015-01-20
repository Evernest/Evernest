using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    internal class UserKeyCreationCommand : CommandBase
    {
        internal readonly long UserId;

        internal readonly string KeyName;

        internal readonly string Key;

        internal UserKeyCreationCommand(SystemCommandHandler systemCommandHandler, long userId, string keyName,
            string key)
            : base(systemCommandHandler)
        {
            UserId = userId;
            KeyName = keyName;
            Key = key;
        }
    }
}
