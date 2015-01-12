
using System;

namespace EvernestFront.Errors
{
    public abstract class AccessDenied : FrontError
    {
        public long StreamId { get; private set; }
        public long UserId { get; private set; }
        public string SourceName { get; private set; }

        protected AccessDenied(long argStreamId, long argUser)
        {
            StreamId = argStreamId;
            UserId = argUser;
            SourceName = null;
        }

        protected AccessDenied(Source src)
        {
            StreamId = src.EventStream.Id;
            UserId = src.User.Id;
            SourceName = src.Name;
        }

        protected AccessDenied() { }
    }
}
