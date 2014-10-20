using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    public class AccessDeniedException : FrontException
    {
        public string StreamName { get; private set; }
        public string User { get; private set; }
        public AccessRights UserRights { get; private set; }
        public AccessRights NeededRights { get; private set; }

        /// <summary>
        /// Constructor for AccessDeniedException.
        /// Synopsis : argUser has rights argUserRights, but argNeededRights are needed : access is denied.
        /// </summary>
        /// <param name="argUser"></param>
        /// <param name="argUserRights"></param>
        /// <param name="argNeededRights"></param>
        public AccessDeniedException(string argUser, AccessRights argUserRights, AccessRights argNeededRights)
        {
            User = argUser;
            UserRights = argUserRights;
            NeededRights = argNeededRights;
        }

    }
}
