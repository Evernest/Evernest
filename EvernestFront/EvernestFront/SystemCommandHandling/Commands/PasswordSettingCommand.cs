using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class PasswordSettingCommand : CommandBase
    {
        internal long UserId { get; private set; }
        
        internal string CurrentPassword { get; private set; }

        internal string NewPassword { get; private set; }

        internal PasswordSettingCommand(SystemCommandHandler systemCommandHandler, long userId,string currentPassword, string newPassword)
            : base(systemCommandHandler)
        {
            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }
}
