
﻿namespace EvernestFront.Responses

{
    public class GetUserResponse : BaseResponse
    {
        public User User { get; private set; }

        internal GetUserResponse(User user)
            : base()
        {
            User = user;
        }

        internal GetUserResponse(FrontError err)
            : base(err)
        {
        }

    }
}
