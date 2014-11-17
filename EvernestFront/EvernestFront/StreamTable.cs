
using System;
using System.Collections.Generic;
using EvernestFront.Errors;

namespace EvernestFront
{
    static class StreamTable
    {
        private static readonly Dictionary<string, Stream> TableByName = new Dictionary<string, Stream>();
        private static readonly Dictionary<Int64, Stream> TableById= new Dictionary<Int64, Stream>();
        /// <summary>
        /// Does nothing if name is available.
        /// Throws a StreamNameTakenException if it is taken.
        /// </summary>
        /// <exception cref="StreamNameTaken"></exception>
        /// <param name="name"></param>
        internal static void CheckNameIsFree(string name)
        {
            if (TableByName.ContainsKey(name))
                throw new StreamNameTaken(name);
        }

        /// <summary>
        /// Adds a new stream called name. Name availability should be checked beforehand !
        /// </summary>
        /// <param name="str"></param>
        
        internal static void Add(Stream str)
        {
            TableByName.Add(str.Name,str);
            TableById.Add(str.Id, str);
        }

        /// <summary>
        /// Returns the stream named name, if it exists.
        /// </summary>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static Stream GetStream(Int64 id)
        {
            if (TableById.ContainsKey(id))
                return TableById[id];
            throw new StreamIdDoesNotExist(id);
        }

        /// <summary>
        /// Returns the name of the stream whose ID is id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        internal static string NameOfId(Int64 id)
        {
            Stream str = GetStream(id);
            return str.Name;
        }

        internal static void Clear()
        {
            TableByName.Clear();
            TableById.Clear();
        }
    }
}
