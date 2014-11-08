
using System.Collections.Generic;
using System.Linq;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    static class RightsTableByStream
    {
        /// <summary>
        /// Access rights table, indexed by stream first, user second.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, AccessRights>> TableByStream
            = new Dictionary<string, Dictionary<string, AccessRights>>();



        /// <summary>
        /// Clears the table.
        /// </summary>
        static internal void Clear()
        {
            TableByStream.Clear();
        }


        /// <summary>
        /// Determines whether the static table contains this stream name.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static bool ContainsStream(string stream)
        {
            return TableByStream.ContainsKey(stream);
        }


        ///// <summary>
        ///// Adds a new stream, with user having rights CreatorRights. Does not check whether the stream name is free.
        ///// </summary>
        ///// <exception cref="StreamNameTakenException"></exception>
        ///// <param name="stream"></param>
        ///// <param name="user"></param>
        //static internal void AddStream(string user, string stream)
        //{
        //    TableByStream[stream] = new Dictionary<string, AccessRights> { { user, Users.CreatorRights } };
        //}

        /// <summary>
        /// Adds a new stream. Does not check whether the stream name is free.
        /// </summary>
        /// <exception cref="StreamNameTakenException"></exception>
        /// <param name="stream"></param>
        static internal void AddStream(string stream)
        {
            TableByStream[stream] = new Dictionary<string, AccessRights>();
        }



        /// <summary>
        /// Sets AccessRights of user about stream to rights. Does not check whether the names are registered.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <param name="rights"></param>
        static internal void SetRights(string user, string stream, AccessRights rights)
        {  
            TableByStream[stream][user] = rights;
            // retirer de la table si rights = AccessRights.NoRights ?
        }

        /// <summary>
        /// Returns the rights of user about stream. Does not check whether the names are registered.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static AccessRights GetRights(string user, string stream)
        {
            var tableAssociatedToStream = TableByStream[stream];
            if (tableAssociatedToStream.ContainsKey(user))
                return tableAssociatedToStream[user];
            else
                return AccessRights.NoRights;
        }

        /// <summary>
        /// Returns a list of all users who have rights on stream, and the associated AccessRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        static internal List<KeyValuePair<string, AccessRights>> UsersOfStream(string stream)
        {
            return TableByStream[stream].ToList();
            //TODO : exclure les users avec droits égaux à NoRights ?
        }


    }
}
