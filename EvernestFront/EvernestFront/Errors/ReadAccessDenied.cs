

using System;

namespace EvernestFront.Errors
{
    public class ReadAccessDenied : AccessDenied
    {

        internal ReadAccessDenied(Int64 stream, Int64 user)
            : base(stream, user) { }

        internal ReadAccessDenied(Source src)
            : base(src) { }
    }
}
