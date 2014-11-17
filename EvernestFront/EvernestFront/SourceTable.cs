using System;
using System.Collections.Generic;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    class SourceTable
    {
        private static readonly Dictionary<String, Source> Table = new Dictionary<String, Source>();

        internal static void AddSource(Source source)
        {
            Table.Add(source.Key,source);
        }

        internal static Source GetSource(String key)
        {
            if (Table.ContainsKey(key))
                return Table[key];
            else throw new SourceKeyDoesNotExistException(key);
        }

        internal static void Clear()
        {
            Table.Clear();
        }
    }
}
