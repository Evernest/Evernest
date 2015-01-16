using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class IdentifyUser : Answer
    {
        public User User { get; private set; }

        internal IdentifyUser(User user)
            : base()
        {
            User = user;
        }

        internal IdentifyUser(FrontError err)
            : base(err)
        {
            
        }
    }
    
}
