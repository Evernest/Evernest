
using System;

namespace EvernestFront.Errors
{
    public abstract class AccessDenied : FrontError
    {
        public Int64 StreamId { get; private set; }
        public Int64 UserId { get; private set; }
        public string SourceName { get; private set; }

        protected AccessDenied(Int64 argStreamId, Int64 argUser)
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
    }
}
