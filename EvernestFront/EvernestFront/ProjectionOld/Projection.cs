using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    class Projection
    {
        internal static Projection Current = new Projection();




        private ImmutableDictionary<long, UserContract> UserTable;

        private ImmutableDictionary<long, EventStreamContract> EventStreamTable;


        private ImmutableDictionary<string, SourceContract> SourceTable;


        private ImmutableDictionary<string, long> UserKeyToId;

        private ImmutableDictionary<string, long> UserNameToId;

        private ImmutableDictionary<string, long> EventStreamNameToId;

        internal Projection()
        {
            UserTable = ImmutableDictionary<long, UserContract>.Empty;
            EventStreamTable = ImmutableDictionary<long, EventStreamContract>.Empty;
            SourceTable = ImmutableDictionary<string, SourceContract>.Empty;
            UserKeyToId = ImmutableDictionary<string, long>.Empty;
            UserNameToId = ImmutableDictionary<string, long>.Empty;
            EventStreamNameToId = ImmutableDictionary<string, long>.Empty;
        }





    }
}
