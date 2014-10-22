using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    static public class Users
    {
        /// <summary>
        /// When adding a new stream, AccessRights to set for its creator
        /// </summary>
        public const AccessRights CreatorRights = AccessRights.Admin;

        /// <summary>
        /// RootUser has rights to do anything. (pas encore implémenté)
        /// </summary>
        public const string RootUser = "RootUser";

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="UserNameTakenException"></exception>
        static public void AddUser(string user)
        {
            RightsTable.AddUser(user);
        }

        /// <summary>
        /// Sets the rights of targetUser for stream streamName to rights. User must have admin rights.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="targetUser"></param>
        /// <param name="rights"></param>
        /// <exception cref="UnregisteredUserException"></exception>
        /// <exception cref="AccessDeniedException"></exception>
        static public void SetRights(string user, string streamName, string targetUser, AccessRights rights)
        {
            // vérifier que user != targetUser ?
                CheckRights.CheckCanAdmin(user, streamName);
                RightsTable.SetRights(targetUser, streamName, rights);
        }

        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnregisteredUserException"></exception>
        static public List<KeyValuePair<string, AccessRights>> StreamsOfUser(string user)
        {
            return RightsTable.StreamsOfUser(user);
        }

        /// <summary>
        /// Returns a list of all users who have rights on stream, and the associated AccessRights. User must have admin rights.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <exception cref="AccessDeniedException"></exception>
        static public List<KeyValuePair<string, AccessRights>> UsersOfStream(string user, string stream)
        {
            CheckRights.CheckCanAdmin(user, stream);
            return RightsTable.UsersOfStream(stream);
        }
    }
}
