

using System;

namespace EvernestFront.Errors
{
    class WriteAccessDenied : AccessDenied
    {

        /// <summary>
        /// Constructor for WriteAccessDeniedException.
        /// Synopsis : user cannot write on stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="rights"></param>

        internal WriteAccessDenied(Int64 stream, Int64 user, AccessRights rights)
            : base(stream, user, rights) { }

        internal WriteAccessDenied(Source src)
            : base(src) { }
    }
}
