﻿namespace EvernestFront.Errors
{
    public class UserKeyDoesNotExist : FrontError
    {
        public string Key { get; private set; }

        internal UserKeyDoesNotExist(string key)
        {
            Key = key;
        }
    }
}
