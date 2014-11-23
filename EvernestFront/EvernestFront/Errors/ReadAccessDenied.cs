

using System;

namespace EvernestFront.Errors
{
    public class ReadAccessDenied : AccessDenied
    {
        /// <summary>
        /// Constructor for ReadAccessDeniedException.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>

        internal ReadAccessDenied(Int64 stream, Int64 user)
            : base(stream, user) { }

        internal ReadAccessDenied(Source src)
            : base(src) { }
    }
}
