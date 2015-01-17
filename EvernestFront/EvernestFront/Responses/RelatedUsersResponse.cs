using System.Collections.Generic;


namespace EvernestFront.Responses
{
    public class RelatedUsersResponse : BaseResponse
    {
        public List<KeyValuePair<long, AccessRight>> Users { get; private set; }


        internal RelatedUsersResponse(List<KeyValuePair<long,AccessRight>> users)
        {
            Users = users;
        }

        internal RelatedUsersResponse(FrontError err)
        :base(err)
        {
            
        }
    }
}
