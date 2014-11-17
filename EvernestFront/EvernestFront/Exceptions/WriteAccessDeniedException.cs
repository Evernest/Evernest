

using System;

namespace EvernestFront.Exceptions
{
    class WriteAccessDeniedException : AccessDeniedException
    {

        /// <summary>
        /// Constructor for WriteAccessDeniedException.
        /// Synopsis : user cannot write on stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="rights"></param>

        internal WriteAccessDeniedException(Int64 stream, Int64 user, AccessRights rights)
            : base(stream, user, rights) { }

        internal WriteAccessDeniedException(Source src)
            : base(src) { }
    }
}
