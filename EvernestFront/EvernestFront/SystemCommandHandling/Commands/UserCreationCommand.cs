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
    }
}
