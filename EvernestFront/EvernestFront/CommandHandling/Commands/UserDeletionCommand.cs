using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.CommandHandling.Commands
{
    class UserDeletionCommand : CommandBase
    {
        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal string Password { get; private set; }

        internal UserDeletionCommand(CommandHandler commandHandler, long userId, string userName, string password)
            : base(commandHandler)
        {
            UserId = userId;
            UserName = userName;
            Password = password;
        }

        public override bool TryToSystemEvent(CommandHandlingData commandHandlingData, out ISystemEvent systemEvent, out FrontError? error)
        {
            CommandHandlingUserData userData;
            if (!commandHandlingData.UserIdToData.TryGetValue(UserId, out userData))
            {
                error=FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(Password, userData.SaltedPasswordHash, userData.PasswordSalt))
            {
                error=FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }
            systemEvent= new UserDeletedSystemEvent(UserName, UserId);
            error = null;
            return true;
        }
    }
}
