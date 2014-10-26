using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    class CheckRights
    {
        /// <summary>
        /// Returns if and only if user can read on stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanRead(string user, string stream)
        {
            var rights = RightsTable.GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                    throw new AccessDeniedException(user, rights, AccessRights.ReadOnly);
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
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
            var rights = RightsTable.GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.ReadOnly):
                    throw new AccessDeniedException(user, rights, AccessRights.ReadWrite);
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
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
            var rights = RightsTable.GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.ReadOnly):
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadWrite):
                    throw new AccessDeniedException(user, rights, AccessRights.Admin);
                case (AccessRights.Admin):
                    return;
            }
        }
    }
}
