

using System;

namespace EvernestFront.Errors
{
    class ReadAccessDenied : AccessDenied
    {
        /// <summary>
        /// Constructor for ReadAccessDeniedException.
        /// Synopsis : user cannot read on stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="userRights"></param>

        internal ReadAccessDenied(Int64 stream, Int64 user, AccessRights userRights)
            : base(stream, user, userRights) { }

        internal ReadAccessDenied(Source src)
            : base(src) { }
    }
}
