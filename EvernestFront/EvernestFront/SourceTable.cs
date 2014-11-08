using System;
using System.Collections.Generic;
using KeyType = System.String; //base 64 int

namespace EvernestFront
{
    class SourceTable
    {
        private static readonly Dictionary<KeyType, Source> Table = new Dictionary<KeyType, Source>();

        private static KeyType NewKey()
        {
            return Keys.NewKey();
        }

        internal static void AddSource(Source source)
        {
            var key = NewKey();
            source.Key = key;
            Table.Add(key,source);
        }

        internal static Source GetSource(KeyType key)
        {
            return Table[key];
        }
    }
}
