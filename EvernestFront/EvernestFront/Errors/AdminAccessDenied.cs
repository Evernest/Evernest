

using System;

namespace EvernestFront.Errors
{
    public class AdminAccessDenied : AccessDenied
    {
        internal AdminAccessDenied(Int64 stream, Int64 user)
            : base(stream, user) { }

        internal AdminAccessDenied(Source src)
            : base(src) { }
    }
}
