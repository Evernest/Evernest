using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public string StreamName { get; private set; }
        public string User { get; private set; }
        public StreamRights UserRights { get; private set; }
        public StreamRights NeededRights { get; private set; }

        public AccessDeniedException(string argUser, StreamRights argUserRights, StreamRights argNeededRights)
        {
            User = argUser;
            UserRights = argUserRights;
            NeededRights = argNeededRights;
        }

    }
}
