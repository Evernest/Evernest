

namespace EvernestFront.Errors
{
    public class AdminAccessDenied : AccessDenied
    {
        internal AdminAccessDenied(long stream, long user)
            : base(stream, user) { }

        internal AdminAccessDenied(Source src)
            : base(src) { }

        internal AdminAccessDenied() { }
    }
}
