namespace EvernestFront.Service.Command
{
    class PasswordSetting : CommandBase
    {
        internal long UserId { get; private set; }
        
        internal string CurrentPassword { get; private set; }

        internal string NewPassword { get; private set; }

    }
}
