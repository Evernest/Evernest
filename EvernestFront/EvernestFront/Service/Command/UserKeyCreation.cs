namespace EvernestFront.Service.Command
{
    class UserKeyCreation : CommandBase
    {
        internal long UserId { get; set; }
        
        internal string KeyName { get; set; }

        internal UserKeyCreation(CommandReceiver commandReceiver, long userId, string keyName)
            :base(commandReceiver)
        {
            UserId = userId;
            KeyName = keyName;
        }
    }
}
