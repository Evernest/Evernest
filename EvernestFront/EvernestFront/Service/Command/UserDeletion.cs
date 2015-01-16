namespace EvernestFront.Service.Command
{
    class UserDeletion : CommandBase
    {
        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal string Password { get; private set; }
    }
}
