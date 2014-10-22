using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    static class RightsTable
    {
        /// <summary>
        /// Access rights table, indexed by stream first, user second.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, AccessRights>> TableByStream 
            = new Dictionary<string, Dictionary<string, AccessRights>>();


        // TODO : historique stocké dans une stream

        /// <summary>
        /// Access rights table, indexed by user first, stream second.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string,AccessRights>> TableByUser
            = new Dictionary<string,Dictionary<string,AccessRights>>();

	//TODO : ajouter de la sûreté entre les noms de string et d'user ?





        static internal void ResetTable()
        {
            TableByUser.Clear();
            TableByStream.Clear();
        }

        /// <summary>
        /// Adds a new user to TableByUser with an empty dictionnary.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="UserNameTakenException"></exception>
        static internal void AddUser(string user)
        {
            if (TableByUser.ContainsKey(user))
                throw new UserNameTakenException(user);
            TableByUser[user] = new Dictionary<string, AccessRights>();
        }

        /// <summary>
        /// Adds a new stream to the static table, with user having rights CreatorRights.
        /// </summary>
        /// <exception cref="StreamNameTakenException"></exception>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        static internal void AddStream(string user, string stream)
        {
            if (!TableByUser.ContainsKey(user))
                throw new UnregisteredUserException(user);
            if (TableByStream.ContainsKey(stream))
                throw new StreamNameTakenException(stream);
            TableByStream[stream] = new Dictionary<string, AccessRights> {{user, Users.CreatorRights}};
            TableByUser[user].Add(stream, Users.CreatorRights);
            // TODO : update la stream historique
        }



        /// <summary>
        /// Set AccessRights of user about stream to rights.
        /// (interdire de destituer un admin ?)
        /// </summary>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <param name="rights"></param>
        static internal void SetRights(string user, string stream, AccessRights rights)
        {
            if (!TableByUser.ContainsKey(user))
                throw new UnregisteredUserException(user);
            if (!TableByStream.ContainsKey(stream))
                throw new StreamNameDoesNotExistException(stream);
            TableByUser[user][stream] = rights;
            TableByStream[stream][user] = rights;
            // retirer des tables si rights = AccessRights.NoRights ?
            // TODO : update la stream historique
            // TODO : interdire de destituer un admin ?
        }

        /// <summary>
        /// Returns the AccessRights of user about stream.
        /// </summary>
        /// <exception cref="UnregisteredUserException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static AccessRights GetRights(string user, string stream)
        {
            if (!TableByUser.ContainsKey(user))
                throw new UnregisteredUserException(user);
            if (!TableByStream.ContainsKey(stream))
                throw new StreamNameDoesNotExistException(stream);

            var tableAssociatedToStream = TableByStream[stream];
            if (tableAssociatedToStream.ContainsKey(user))
                return tableAssociatedToStream[user];
            else
                return AccessRights.NoRights;
            // vérifier la cohérence avec l'autre table ?
        }


        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnregisteredUserException"></exception>
        static internal List<KeyValuePair<string, AccessRights>> StreamsOfUser(string user)
        {
            if (!TableByUser.ContainsKey(user))
                throw new UnregisteredUserException(user);
            return TableByUser[user].ToList();
            //TODO : exclure les streams avec droits égaux à NoRights ?
        }

        /// <summary>
        /// Returns a list of all users who have rights on stream, and the associated AccessRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        static internal List<KeyValuePair<string, AccessRights>> UsersOfStream(string stream)
        {
            if (!TableByStream.ContainsKey(stream))
                throw new StreamNameDoesNotExistException(stream);
            return TableByStream[stream].ToList();
            //TODO : exclure les users avec droits égaux à NoRights ?
        }
    }
}
