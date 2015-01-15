

namespace EvernestFront.Answers
{
    public class SetPassword : Answer
    {
        public long User { get; private set; }
        public string NewPassword { get; private set; }

        internal SetPassword(long user, string password)
            : base()
        {
            User = user;
            NewPassword = password;
        }

        internal SetPassword(FrontError err)
            : base(err)
        {
            
        }

    }
}
