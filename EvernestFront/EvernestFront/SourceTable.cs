using System;
using System.Collections.Generic;


namespace EvernestFront
{
    class SourceTable
    {
        private static readonly Dictionary<String, Source> Table = new Dictionary<String, Source>();

        internal static bool SourceKeyExists(String key)
        {
            return Table.ContainsKey(key);
        }

        internal static void AddSource(Source source)
        {
            Table.Add(source.Key,source);
        }

        /// <summary>
        /// Key existence should be checked beforehand !
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static Source GetSource(String key)
        {
            return Table[key];
        }

        internal static void Clear()
        {
            Table.Clear();
        }
    }
}
