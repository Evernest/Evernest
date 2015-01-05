

using System;

namespace EvernestFront.Errors
{
    public class WriteAccessDenied : AccessDenied
    {
        internal WriteAccessDenied(long stream, long user)
            : base(stream, user) { }

        internal WriteAccessDenied(Source src)
            : base(src) { }
    }
}
