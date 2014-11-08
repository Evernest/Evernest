

namespace EvernestFront.Exceptions
{
    class ReadAccessDeniedException : AccessDeniedException
    {
        /// <summary>
        /// Constructor for ReadAccessDeniedException.
        /// Synopsis : user cannot read on stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="userRights"></param>

        internal ReadAccessDeniedException(string stream, string user, AccessRights userRights)
            : base(stream, user, userRights) { }
    }
}
