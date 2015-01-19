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


        public override bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!systemCommandHandlerState.UserIdToData.TryGetValue(UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent =null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(CurrentPassword, userRecord.SaltedPasswordHash, userRecord.PasswordSalt))
            {
                error=FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }

            var hashSalt = passwordManager.SaltAndHash(NewPassword);
            systemEvent = new PasswordSetSystemEvent(UserId, hashSalt.Item1, hashSalt.Item2);
            error = null;
            return true;
        }        
    }
}
