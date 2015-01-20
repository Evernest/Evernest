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
    }
}
