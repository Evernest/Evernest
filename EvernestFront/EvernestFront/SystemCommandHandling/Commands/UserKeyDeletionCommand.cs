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
    }
}
