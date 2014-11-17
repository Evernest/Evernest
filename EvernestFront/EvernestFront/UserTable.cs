using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront
{
    class UserTable
    {
        private static readonly Dictionary<string, User> TableByName = new Dictionary<string, User>();
        private static readonly Dictionary<Int64, User> TableById = new Dictionary<Int64, User>();

        /// <summary>
        /// Does nothing if name is available.
        /// Throws a UserNameTakenException if it is taken.
        /// </summary>
        /// <exception cref="UserNameTaken"></exception>
        /// <param name="name"></param>
        internal static void CheckNameIsFree(string name)
        {
            if (TableByName.ContainsKey(name))
                throw new UserNameTaken(name);
        }

        /// <summary>
        /// Adds a new user. Username availability should be checked beforehand !
        /// </summary>
        /// <param name="usr"></param>
        internal static void Add(User usr)
        {
            TableByName.Add(usr.Name, usr);
            TableById.Add(usr.Id, usr);
        }

        /// <summary>
        /// Gets user whose ID is id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserIdDoesNotExist"></exception>
        internal static User GetUser(Int64 id)
        {
            if (TableById.ContainsKey(id))
                return TableById[id];
            else
            {
                throw new UserIdDoesNotExist(id);
            }
        }

        /// <summary>
        /// Returns the name of the user whose ID is id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static string NameOfId(Int64 id)
        {
            User usr = GetUser(id);
            return usr.Name;
        }

        internal static void Clear()
        {
            TableByName.Clear();
            TableById.Clear();
        }


    }
}
