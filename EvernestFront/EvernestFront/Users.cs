using System;
using System.Collections.Generic;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    static public class Users
    {
        /// <summary>
        /// When adding a new stream, AccessRights to set for its creator.
        /// </summary>
        public const AccessRights CreatorRights = AccessRights.Admin;

        /// <summary>
        /// RootUser has rights to do anything. (/!\ pas encore implémenté)
        /// </summary>
        public const string RootUser = "RootUser";


        /// <summary>
        /// Sets the rights of targetUser for stream streamName to rights. User must have admin rights.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <param name="targetUserId"></param>
        /// <param name="rights"></param>
        /// <exception cref="UserIdDoesNotExistException"></exception>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        static internal void SetRights(Int64 userId, Int64 streamId, Int64 targetUserId, AccessRights rights)
        {
            var user = UserTable.GetUser(userId);
            var stream = StreamTable.GetStream(streamId);
            var targetUser = UserTable.GetUser(targetUserId);
            CheckRights.CheckCanAdmin(user, stream);
            CheckRights.CheckRightsCanBeModified(targetUser, stream);
            UserRight.SetRight(targetUser, stream, rights);        
            // TODO : update la stream historique
        }

        /// <summary>
        /// Returns the rights of user about stream.
        /// </summary>
        /// <exception cref="UserIdDoesNotExistException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <param name="streamId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static AccessRights GetRights(Int64 userId, Int64 streamId)
        {
            var user = UserTable.GetUser(userId);
            var stream = StreamTable.GetStream(streamId);
            return UserRight.GetRight(user, stream);
        }
    }
}
