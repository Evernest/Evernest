
using System;

namespace EvernestFront.Errors
{
    public abstract class AccessDenied : FrontError
    {
        public Int64 StreamId { get; private set; }
        public Int64 User { get; private set; }
        public Int64? SourceId { get; private set; }

        /// <summary>
        /// Constructor for AccessDeniedException.
        /// </summary>
        /// <param name="argStreamId"></param>
        /// <param name="argUser"></param>
        
        protected AccessDenied(Int64 argStreamId, Int64 argUser)
        {
            StreamId = argStreamId;
            User = argUser;
            SourceId = null;
        }

        protected AccessDenied(Source src)
        {
            StreamId = src.Stream.Id;
            User = src.User.Id;
            SourceId = src.Id;
        }
    }
}
