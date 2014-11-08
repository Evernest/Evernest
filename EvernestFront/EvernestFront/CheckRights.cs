
using EvernestFront.Exceptions;

namespace EvernestFront
{
    static class CheckRights
    {
        static private AccessRights GetRights(string user, string stream)
        {
            return RightsTableByUser.GetRights(user, stream);
            // sécurité ?
        }



        /// <summary>
        /// Returns if and only if user can read on stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanRead(string user, string stream)
        {
            var rights = GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                    throw new ReadAccessDeniedException(stream, user, rights);
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return;
            }
        }

        /// <summary>
        /// Returns if and only if user can write on stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanWrite(string user, string stream)
        {
            var rights = GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.ReadOnly):
                    throw new WriteAccessDeniedException(stream, user, rights);
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return;
            }
        }

        /// <summary>
        /// Returns if and only if user can administrate stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanAdmin(string user, string stream)
        {
            var rights = GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.ReadOnly):
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadWrite):
                    throw new AdminAccessDeniedException(stream, user, rights);
                case (AccessRights.Admin):
                case (AccessRights.Root):    
                    return;
            }
        }

        static internal void CheckRightsCanBeModified(string user, string stream)
        {
            var rights = GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    throw new CannotDestituteAdminException(stream, user);
                case (AccessRights.NoRights):
                case (AccessRights.ReadOnly):
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadWrite):
                    return;
            }
        }

    }
}
