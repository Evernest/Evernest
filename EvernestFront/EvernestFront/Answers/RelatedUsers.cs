using System.Collections.Generic;


namespace EvernestFront.Answers
{
    public class RelatedUsers : Answer
    {
        public List<KeyValuePair<long, AccessRight>> Users { get; private set; }

        internal RelatedUsers(List<KeyValuePair<long,AccessRight>> users)
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
