

using System;

namespace EvernestFront.Exceptions
{
    class ReadAccessDeniedException : AccessDeniedException
    {
        /// <summary>
        /// Constructor for ReadAccessDeniedException.
        /// Synopsis : user cannot read on stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="userRights"></param>

        internal ReadAccessDeniedException(Int64 stream, Int64 user, AccessRights userRights)
            : base(stream, user, userRights) { }

        internal ReadAccessDeniedException(Source src)
            : base(src) { }
    }
}
