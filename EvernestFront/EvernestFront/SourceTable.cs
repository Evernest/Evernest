using System;
using System.Collections.Generic;
using KeyType = System.String; //base 64 int
using EvernestFront.Exceptions;

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
            if (Table.ContainsKey(key))
                return Table[key];
            else throw new Exception("SourceKeyDoesNotExist"); // exceptions to be removed soon
        }

        internal static void Clear()
        {
            Table.Clear();
        }
    }
}
