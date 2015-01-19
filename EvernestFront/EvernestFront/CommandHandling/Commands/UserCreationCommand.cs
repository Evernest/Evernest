using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.CommandHandling.Commands
{
    class UserCreationCommand : CommandBase
    {
        internal string UserName { get; private set; }

        internal string Password { get; private set; }

        internal UserCreationCommand(CommandHandler commandHandler, string userName, string password)
            :base(commandHandler)
        {
            UserName = userName;
            Password = password;
        }

        public override bool TryToSystemEvent(CommandHandlingData commandHandlingData, out ISystemEvent systemEvent, out FrontError? error)
        {
            if (commandHandlingData.UserNames.Contains(UserName))
            {
                error=FrontError.UserNameTaken;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            var hashSalt = passwordManager.SaltAndHash(Password);
            systemEvent= new UserCreatedSystemEvent(UserName, commandHandlingData.NextUserId, hashSalt.Key, hashSalt.Value);
            error = null;
            return true;
        }
    }
}
