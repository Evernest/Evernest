using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Request
{
    class SetRights:Request
    {
        private string targetUser;
        private StreamRights rights;

        public SetRights(string user, string streamName, string targetUser, StreamRights rights)
            : base(user, streamName)
        {
            this.targetUser = targetUser;
            this.rights = rights;
        }
        
        public override IAnswer Process()
        {
            throw new NotImplementedException();
        }
    }
}
