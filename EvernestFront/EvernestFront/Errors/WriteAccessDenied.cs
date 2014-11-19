

using System;

namespace EvernestFront.Errors
{
    class WriteAccessDenied : AccessDenied
    {

        /// <summary>
        /// Constructor for WriteAccessDeniedException.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>

        internal WriteAccessDenied(Int64 stream, Int64 user)
            : base(stream, user) { }

        internal WriteAccessDenied(Source src)
            : base(src) { }
    }
}
