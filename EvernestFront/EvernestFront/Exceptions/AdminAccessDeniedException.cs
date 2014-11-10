

using System;

namespace EvernestFront.Exceptions
{
    class AdminAccessDeniedException : AccessDeniedException
    {
        /// <summary>
        /// Constructor for AdminAccessDeniedException.
        /// Synopsis : user cannot administrate stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="userRights"></param>
        /// 
        internal AdminAccessDeniedException(Int64 stream, Int64 user, AccessRights userRights)
            : base(stream, user, userRights) { }
    }
}
