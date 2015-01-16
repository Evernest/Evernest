namespace EvernestFront.Service.Command
{
    class UserCreation : CommandBase
    {
        internal string UserName { get; private set; }

        internal string Password { get; private set; }
    }
}
