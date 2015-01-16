namespace EvernestFront.Service.Command
{
    class UserKeyCreation : CommandBase
    {
        internal long UserId { get; set; }
        
        internal string KeyName { get; set; }
    }
}
