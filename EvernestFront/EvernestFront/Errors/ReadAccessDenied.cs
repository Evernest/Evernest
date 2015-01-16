

namespace EvernestFront.Errors
{
    public class ReadAccessDenied : AccessDenied
    {

        internal ReadAccessDenied(long stream, long user)
            : base(stream, user) { }

        internal ReadAccessDenied(Source src)
            : base(src) { }
    }
}
