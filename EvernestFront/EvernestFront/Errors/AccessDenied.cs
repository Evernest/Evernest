
using System;

namespace EvernestFront.Errors
{
    public abstract class AccessDenied : FrontError
    {
        public Int64 StreamId { get; private set; }
        public Int64 User { get; private set; }
        public Int64? SourceId { get; private set; }

        protected AccessDenied(Int64 argStreamId, Int64 argUser)
        {
            StreamId = argStreamId;
            User = argUser;
            SourceId = null;
        }

        protected AccessDenied(Source src)
        {
            StreamId = src.EventStream.Id;
            User = src.User.Id;
            SourceId = src.Id;
        }
    }
}
