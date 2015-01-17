using System.Collections.Generic;


namespace EvernestFront.Responses
{
    public class RelatedUsersResponse : BaseResponse
    {
        public List<KeyValuePair<long, AccessRights>> Users { get; private set; }

        internal RelatedUsersResponse(List<KeyValuePair<long,AccessRights>> users)
            :base()
        {
            Users = users;
        }

        internal RelatedUsersResponse(FrontError err)
        :base(err)
        {
            
        }
    }
}
