using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Requests
{
    class SetRights:Request
    {
        private string targetUser;
        private AccessRights rights;

        public SetRights(string user, string streamName, string targetUser, AccessRights rights)
            : base(user, streamName)
        {
            this.targetUser = targetUser;
            this.rights = rights;
        }
        
        public override Answers.IAnswer Process()
        {
            throw new NotImplementedException();
        }
    }
}
