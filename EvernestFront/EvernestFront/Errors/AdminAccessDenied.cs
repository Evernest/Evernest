

using System;

namespace EvernestFront.Errors
{
    class AdminAccessDenied : AccessDenied
    {
        /// <summary>
        /// Constructor for AdminAccessDeniedException.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        internal AdminAccessDenied(Int64 stream, Int64 user)
            : base(stream, user) { }

        internal AdminAccessDenied(Source src)
            : base(src) { }
    }
}
