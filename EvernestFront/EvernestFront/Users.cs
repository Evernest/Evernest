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
        /// Registers a new user and returns its ID.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="UserNameTakenException"></exception>
        static public Int64 AddUser(string user)
        {
            UserTable.CheckNameIsFree(user);
            var usr=new User(user);
            UserTable.Add(usr);
            return usr.Id;
        }



        /// <summary>
        /// Sets the rights of targetUser for stream streamName to rights. User must have admin rights.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <param name="targetUser"></param>
        /// <param name="rights"></param>
        /// <exception cref="UnregisteredUserException"></exception>
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
        /// <exception cref="UnregisteredUserException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <param name="stream"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static AccessRights GetRights(Int64 userId, Int64 streamId)
        {
            var user = UserTable.GetUser(userId);
            var stream = StreamTable.GetStream(streamId);
            return UserRight.GetRight(user, stream);
            // vérifier la cohérence avec l'autre table ?
        }


        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnregisteredUserException"></exception>
        static public List<KeyValuePair<string, AccessRights>> StreamsOfUser(Int64 user)
        {
            throw new NotImplementedException();
            //if (!RightsTableByUser.ContainsUser(user))
            //    throw new UnregisteredUserException(user);
            //return RightsTableByUser.StreamsOfUser(user);

            //TODO : exclure les streams avec droits égaux à NoRights ?
        }

        /// <summary>
        /// Returns a list of all users who have rights on stream, and the associated AccessRights. User must have admin rights.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="AccessDeniedException"></exception>
        static public List<KeyValuePair<string, AccessRights>> UsersOfStream(Int64 user, Int64 stream)
        {
            throw new NotImplementedException();

            //if (!RightsTableByStream.ContainsStream(stream))
            //    throw new StreamIdDoesNotExistException(stream);
            //CheckRights.CheckCanAdmin(user, stream);
            //return RightsTableByStream.UsersOfStream(stream);

            //TODO : exclure les users avec droits égaux à NoRights ?
        }
    }
}
