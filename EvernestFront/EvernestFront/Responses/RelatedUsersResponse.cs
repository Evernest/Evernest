using System.Collections.Generic;


namespace EvernestFront.Responses
{
    public class RelatedUsersResponse : BaseResponse
    {
        public List<KeyValuePair<string, AccessRight>> Users { get; private set; }


        internal RelatedUsersResponse(List<KeyValuePair<string,AccessRight>> users)
        {
            Users = users;
        }

        internal RelatedUsersResponse(FrontError err)
        :base(err)
        {
            
        }
    }
}
