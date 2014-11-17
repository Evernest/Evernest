
using System;

namespace EvernestFront.Errors
{
    public abstract class AccessDenied : FrontError
    {
        public Int64 StreamId { get; private set; }
        public Int64 User { get; private set; }
        public AccessRights UserRights { get; private set; }
        public Int64? SourceId { get; private set; }

        /// <summary>
        /// Constructor for AccessDeniedException.
        /// </summary>
        /// <param name="argStreamId"></param>
        /// <param name="argUser"></param>
        /// <param name="argUserRights"></param>
        
        protected AccessDenied(Int64 argStreamId, Int64 argUser, AccessRights argUserRights)
        {
            StreamId = argStreamId;
            User = argUser;
            UserRights = argUserRights;
            SourceId = null;
        }

        protected AccessDenied(Source src)
        {
            StreamId = src.Stream.Id;
            User = src.User.Id;
            UserRights = src.Right;
            SourceId = src.Id;
        }
    }
}
