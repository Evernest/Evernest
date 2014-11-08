
using System.Collections.Generic;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    static class StreamTable
    {
        private static readonly Dictionary<string, Stream> Table = new Dictionary<string, Stream>();

        /// <summary>
        /// Does nothing if name is available.
        /// Throws a StreamNameTakenException if it is taken.
        /// </summary>
        /// <exception cref="StreamNameTakenException"></exception>
        /// <param name="name"></param>
        internal static void CheckNameIsFree(string name)
        {
            if (Table.ContainsKey(name))
                throw new StreamNameTakenException(name);
        }

        /// <summary>
        /// Adds a new stream called name if name is available.
        /// </summary>
        /// <exception cref="StreamNameTakenException"></exception>
        /// <param name="name"></param>
        /// <param name="str"></param>
        
        internal static void Add(string name, Stream str)
        {
            if (Table.ContainsKey(name))
                throw new StreamNameTakenException(name);
            Table.Add(name, str);
        }

        /// <summary>
        /// Returns the stream named name, if it exists.
        /// </summary>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static Stream GetStream(string name)
        {
            if (Table.ContainsKey(name))
                return Table[name];
            throw new StreamNameDoesNotExistException(name);
        }
    }
}
