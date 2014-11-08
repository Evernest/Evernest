

namespace EvernestFront.Exceptions
{
    class AdminAccessDeniedException : AccessDeniedException
    {
        /// <summary>
        /// Constructor for AdminAccessDeniedException.
        /// Synopsis : user cannot administrate stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="userRights"></param>
        /// 
        internal AdminAccessDeniedException(string stream, string user, AccessRights userRights)
            : base(stream, user, userRights) { }
    }
}
