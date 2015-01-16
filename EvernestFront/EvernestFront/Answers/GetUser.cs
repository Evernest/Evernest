
﻿namespace EvernestFront.Answers

{
    public class GetUser : Answer
    {
        public User User;

        internal GetUser(User user)
            : base()
        {
            User = user;
        }

        internal GetUser(FrontError err)
            : base(err)
        {
        }

    }
}
