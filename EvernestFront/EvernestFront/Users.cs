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
        /// When adding a new stream, AccessRights to set for its creator.
        /// </summary>
        public const AccessRights CreatorRights = AccessRights.Admin;

        /// <summary>
        /// RootUser has rights to do anything. (/!\ pas encore implémenté)
        /// </summary>
        public const string RootUser = "RootUser";


        static void ClearTables()
        {
            RightsTableByUser.Clear();
            RightsTableByStream.Clear();
            RightsTableByUser.AddUser(RootUser);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="UserNameTakenException"></exception>
        static public void AddUser(string user)
        {
            if (RightsTableByUser.ContainsUser(user))
                throw new UserNameTakenException(user);
            RightsTableByUser.AddUser(user);
            // TODO : update la stream historique
        }


        /// <summary>
        /// Adds a new stream, about which user has CreatorRights.
        /// </summary>
        /// <exception cref="UnregisteredUserException"></exception>
        /// <exception cref="StreamNameTakenException"></exception>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        static public void AddStream(string user, string stream)
        {
            if (!RightsTableByUser.ContainsUser(user))
                throw new UnregisteredUserException(user);
            if (RightsTableByStream.ContainsStream(stream))
                throw new StreamNameTakenException(stream);
            RightsTableByStream.AddStream(stream);
            RightsTableByStream.SetRights(user, stream, CreatorRights);
            RightsTableByUser.SetRights(user, stream, CreatorRights);
            // TODO : update la stream historique
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
        static public void SetRights(string user, string stream, string targetUser, AccessRights rights)
        {
            if (!RightsTableByUser.ContainsUser(user))
                throw new UnregisteredUserException(user);
            if (!RightsTableByStream.ContainsStream(stream))
                throw new StreamNameDoesNotExistException(stream);
            if (!RightsTableByUser.ContainsUser(targetUser))
                throw new UnregisteredUserException(targetUser);
            CheckRights.CheckCanAdmin(user, stream);
            CheckRights.CheckRightsCanBeModified(targetUser, stream);
            RightsTableByUser.SetRights(targetUser, stream, rights);
            RightsTableByStream.SetRights(targetUser, stream, rights);          
            // TODO : update la stream historique
        }

        /// <summary>
        /// Returns the rights of user about stream.
        /// </summary>
        /// <exception cref="UnregisteredUserException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static AccessRights GetRights(string user, string stream)
        {
            if (!RightsTableByUser.ContainsUser(user))
                throw new UnregisteredUserException(user);
            if (!RightsTableByStream.ContainsStream(stream))
                throw new StreamNameDoesNotExistException(stream);
            return RightsTableByUser.GetRights(user, stream);
            // vérifier la cohérence avec l'autre table ?
        }


        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnregisteredUserException"></exception>
        static public List<KeyValuePair<string, AccessRights>> StreamsOfUser(string user)
        {
            if (!RightsTableByUser.ContainsUser(user))
                throw new UnregisteredUserException(user);
            return RightsTableByUser.StreamsOfUser(user);
            //TODO : exclure les streams avec droits égaux à NoRights ?
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
            if (!RightsTableByStream.ContainsStream(stream))
                throw new StreamNameDoesNotExistException(stream);
            CheckRights.CheckCanAdmin(user, stream);
            return RightsTableByStream.UsersOfStream(stream);
            //TODO : exclure les users avec droits égaux à NoRights ?
        }
    }
}
