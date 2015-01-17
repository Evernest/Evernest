using System.Collections.Generic;


namespace EvernestFront.Responses
{
    public class RelatedUsersResponse : BaseResponse
    {
        public List<KeyValuePair<long, AccessRight>> Users { get; private set; }

<<<<<<< HEAD:EvernestFront/EvernestFront/Answers/RelatedUsers.cs
        internal RelatedUsers(List<KeyValuePair<long,AccessRight>> users)
=======
        internal RelatedUsersResponse(List<KeyValuePair<long,AccessRights>> users)
>>>>>>> origin/RemoveContractsOverProjection:EvernestFront/EvernestFront/Responses/RelatedUsersResponse.cs
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
