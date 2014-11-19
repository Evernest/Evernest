using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace EvernestFront.Projection
{
    class Tables
    {
        readonly ImmutableDictionary<KeyValuePair<long, long>, AccessRights> RightTable;
        readonly ImmutableDictionary<long, List<long>> UsersOfStream;
        readonly ImmutableDictionary<long, List<long>> StreamsOfUser;
    }
}
