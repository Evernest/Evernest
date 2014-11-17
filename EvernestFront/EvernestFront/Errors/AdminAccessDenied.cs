

using System;

namespace EvernestFront.Errors
{
    class AdminAccessDenied : AccessDenied
    {
        /// <summary>
        /// Constructor for AdminAccessDeniedException.
        /// Synopsis : user cannot administrate stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="userRights"></param>
        /// 
        internal AdminAccessDenied(Int64 stream, Int64 user, AccessRights userRights)
            : base(stream, user, userRights) { }

        internal AdminAccessDenied(Source src)
            : base(src) { }
    }
}
