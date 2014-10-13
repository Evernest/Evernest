using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    static class StreamTable
    {
        private static readonly Dictionary<string, Stream> Table = new Dictionary<string, Stream>();

        internal static void CheckNameIsFree(string name)
        {
            if (Table.ContainsKey(name))
                throw new StreamNameTakenException(name);
        }

        internal static void Add(string name, Stream str)
        {
            if (Table.ContainsKey(name))
                throw new StreamNameTakenException(name);
            Table.Add(name, str);
        }

        internal static Stream GetStream(string name)
        {
            if (Table.ContainsKey(name))
                return Table[name];
            throw new StreamNameDoesNotExistException(name);
        }
    }
}
