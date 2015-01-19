using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class UserDeletionCommand : CommandBase
    {
        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal string Password { get; private set; }

        internal UserDeletionCommand(SystemCommandHandler systemCommandHandler, long userId, string userName, string password)
            : base(systemCommandHandler)
        {
            UserId = userId;
            UserName = userName;
            Password = password;
        }

        public override bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!systemCommandHandlerState.UserIdToData.TryGetValue(UserId, out userRecord))
            {
                error=FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(Password, userRecord.SaltedPasswordHash, userRecord.PasswordSalt))
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
