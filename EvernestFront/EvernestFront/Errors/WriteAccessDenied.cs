

using System;

namespace EvernestFront.Errors
{
    public class WriteAccessDenied : AccessDenied
    {
        internal WriteAccessDenied(Int64 stream, Int64 user)
            : base(stream, user) { }

        internal WriteAccessDenied(Source src)
            : base(src) { }
    }
}
