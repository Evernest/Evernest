

namespace EvernestFront.Responses
{
    public class IdentifyUserResponse : BaseResponse
    {
        public User User { get; private set; }

        internal IdentifyUserResponse(User user)
            : base()
        {
            User = user;
        }

        internal IdentifyUserResponse(FrontError err)
            : base(err)
        {
            
        }
    }
    
}
