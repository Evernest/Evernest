using System.Collections.Generic;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class RelatedUsers : Answer
    {
        public List<KeyValuePair<long, AccessRights>> Users { get; private set; }

        internal RelatedUsers(List<KeyValuePair<long,AccessRights>> users)
            :base()
        {
            Users = users;
        }

        internal RelatedUsers(FrontError err)
        :base(err)
        {
            
        }
    }
}
