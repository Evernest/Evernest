using System;
using System.Collections.Generic;
using KeyType = System.String; //base 64 int

namespace EvernestFront
{
    class SourceTable
    {
        private static readonly Dictionary<KeyType, Source> Table = new Dictionary<KeyType, Source>();

        internal static void AddSource(Source source)
        {
            Table.Add(source.Key,source);
        }

        internal static Source GetSource(KeyType key)
        {
            return Table[key];
        }

        internal static void Clear()
        {
            Table.Clear();
        }
    }
}
