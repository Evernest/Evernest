using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.CommandHandling.Commands
{
    class PasswordSettingCommand : CommandBase
    {
        internal long UserId { get; private set; }
        
        internal string CurrentPassword { get; private set; }

        internal string NewPassword { get; private set; }

        internal PasswordSettingCommand(CommandHandler commandHandler, long userId,string currentPassword, string newPassword)
            : base(commandHandler)
        {
            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }


        public override bool TryToSystemEvent(CommandHandlingData commandHandlingData, out ISystemEvent systemEvent, out FrontError? error)
        {
            CommandHandlingUserData userData;
            if (!commandHandlingData.UserIdToData.TryGetValue(UserId, out userData))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent =null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(CurrentPassword, userData.SaltedPasswordHash, userData.PasswordSalt))
            {
                error=FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }

            var hashSalt = passwordManager.SaltAndHash(NewPassword);
            systemEvent = new PasswordSetSystemEvent(UserId, hashSalt.Key, hashSalt.Value);
            error = null;
            return true;
        }        
    }
}
