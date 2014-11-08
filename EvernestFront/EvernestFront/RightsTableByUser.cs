using System.Collections.Generic;
using System.Linq;

namespace EvernestFront
{
    static class RightsTableByUser
    {
        /// <summary>
        /// Access rights table, indexed by user first, stream second.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, AccessRights>> TableByUser
            = new Dictionary<string, Dictionary<string, AccessRights>>();

        /// <summary>
        /// Clears the table.
        /// </summary>
        static internal void Clear()
        {
            TableByUser.Clear();
        }

        /// <summary>
        /// Determines whether the static table contains this user name.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool ContainsUser (string user)
        {
            return TableByUser.ContainsKey(user);
        }


        /// <summary>
        /// Adds a new user. Does not check whether the name is free.
        /// </summary>
        /// <param name="user"></param>
        static internal void AddUser(string user)
        {
            TableByUser[user] = new Dictionary<string, AccessRights>();
        }

        /// <summary>
        /// Sets AccessRights of user about stream to rights. Does not check whether the names are registered.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <param name="rights"></param>
        static internal void SetRights(string user, string stream, AccessRights rights)
        {
            TableByUser[user][stream] = rights;
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
            var tableAssociatedToUser = TableByUser[user];
            if (tableAssociatedToUser.ContainsKey(stream))
                return tableAssociatedToUser[stream];
            else
                return AccessRights.NoRights;
        }

        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated rights.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        static internal List<KeyValuePair<string, AccessRights>> StreamsOfUser(string user)
        {
            return TableByUser[user].ToList();
            //TODO : exclure les streams avec droits égaux à NoRights ?
        }
    }
}
