namespace EvernestFront.Errors
{
    class UserKeyNameTaken : FrontError
    {
        //user UserId already owns a key called KeyName, he cannot create another one.

        public long UserId { get; private set; }
        public string KeyName { get; private set; }

        internal UserKeyNameTaken(long userId, string keyName)
        {
            UserId = userId;
            KeyName = keyName;
        }
    }
}
