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
        public AccessRights UserRights { get; private set; }
        public AccessRights NeededRights { get; private set; }

        public AccessDeniedException(string argUser, AccessRights argUserRights, AccessRights argNeededRights)
        {
            User = argUser;
            UserRights = argUserRights;
            NeededRights = argNeededRights;
        }

    }
}
