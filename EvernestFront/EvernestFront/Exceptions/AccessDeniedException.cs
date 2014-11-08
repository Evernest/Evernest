
namespace EvernestFront.Exceptions
{
    public abstract class AccessDeniedException : FrontException
    {
        public string StreamName { get; private set; }
        public string User { get; private set; }
        public AccessRights UserRights { get; private set; }

        /// <summary>
        /// Constructor for AccessDeniedException.
        /// </summary>
        /// <param name="argStreamName"></param>
        /// <param name="argUser"></param>
        /// <param name="argUserRights"></param>
        
        protected AccessDeniedException(string argStreamName, string argUser, AccessRights argUserRights)
        {
            StreamName = argStreamName;
            User = argUser;
            UserRights = argUserRights;
        }

    }
}
