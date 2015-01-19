using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class UserCreationCommand : CommandBase
    {
        internal string UserName { get; private set; }

        internal string Password { get; private set; }

        internal UserCreationCommand(SystemCommandHandler systemCommandHandler, string userName, string password)
            :base(systemCommandHandler)
        {
            UserName = userName;
            Password = password;
        }

        public override bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error)
        {
            if (systemCommandHandlerState.UserNames.Contains(UserName))
            {
                error=FrontError.UserNameTaken;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            var hashSalt = passwordManager.SaltAndHash(Password);
            systemEvent= new UserCreatedSystemEvent(UserName, systemCommandHandlerState.NextUserId, hashSalt.Item1, hashSalt.Item2);
            error = null;
            return true;
        }
    }
}
