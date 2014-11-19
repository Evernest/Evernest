
using System;
using System.Collections.Generic;
using EvernestFront.Errors;

namespace EvernestFront
{
    static class StreamTable
    {
        private static readonly Dictionary<string, Stream> TableByName = new Dictionary<string, Stream>();
        private static readonly Dictionary<Int64, Stream> TableById= new Dictionary<Int64, Stream>();
      
        internal static bool NameIsFree(string name)
        {
            return (!TableByName.ContainsKey(name));
        }

        public static bool StreamIdExists(long streamId)
        {
            return TableById.ContainsKey(streamId);
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
        /// Returns the stream with ID id. Stream ID existence should be checked beforehand !
        /// </summary>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static Stream GetStream(Int64 id)
        {
                return TableById[id];
        }

        /// <summary>
        /// Returns the name of the stream whose ID is id. Stream ID existence should be checked beforehand !
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
