using System.Collections.Generic;
using KeyType = System.Int64;

namespace EvernestFront
{
    class SourceTable
    {
        private static readonly Dictionary<KeyType, Source> Table = new Dictionary<KeyType, Source>();

        private static KeyType _nextKey=0;
        private static KeyType NewKey()
        {
            _nextKey++;
            return _nextKey;
        }
        // à améliorer

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
