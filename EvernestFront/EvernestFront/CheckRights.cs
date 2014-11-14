
using System.Data.SqlTypes;
using EvernestFront.Exceptions;
using System;

namespace EvernestFront
{
    //Checking stream and user existence should be done before calling anything from this class.

    static class CheckRights
    {
        //static private AccessRights GetRights(string user, string stream)
        //{
        //    return RightsTableByUser.GetRights(user, stream);
        //    // sécurité ?
        //}



        static bool CanRead(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                    return false;
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return true;
                default:
                    throw new Exception("CheckRights.CanRead : cas non traité");
            }
        }

        static bool CanWrite(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.ReadOnly):
                    return false;
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return true;
                default:
                    throw new Exception("CheckRights.CanWrite : cas non traité");
            }
        }

        static bool CanAdmin(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                    return false;
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return true;
                default:
                    throw new Exception("CheckRights.CanAdmin : cas non traité");
            }
        }

        static bool CanBeModified(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                    return true;
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return false;
                default:
                    throw new Exception("CheckRights.CanAdmin : cas non traité");
            }
        }
        

        /// <summary>
        /// Returns if and only if user can read on stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="ReadAccessDeniedException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanRead(User user, Stream stream)
        {
            var rights = UserRight.GetRight(user, stream);
            if (CanRead(rights))
                return;
            else
                throw new ReadAccessDeniedException(stream.Id, user.Id, rights);
        }

        /// <summary>
        /// Returns if and only if user can write on stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="WriteAccessDeniedException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanWrite(User user, Stream stream)
        {
            var rights = UserRight.GetRight(user, stream);
            if (CanWrite(rights))
                return;
            else
                throw new WriteAccessDeniedException(stream.Id, user.Id, rights);
        }

        /// <summary>
        /// Returns if and only if user can administrate stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="AdminAccessDeniedException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanAdmin(User user, Stream stream)
        {
            var rights = UserRight.GetRight(user, stream);
            if (CanAdmin(rights))
                return;
            else
                throw new AdminAccessDeniedException(stream.Id, user.Id, rights);
        }

        static internal void CheckRightsCanBeModified(User user, Stream stream)
        {
            var rights = UserRight.GetRight(user, stream);
            if (CanBeModified(rights))
                return;
            else
                throw new CannotDestituteAdminException(stream.Id, user.Id);
        }

    }
}
