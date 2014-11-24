using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract
{
    [DataContract]
    class StreamContract
    {
        [DataMember]
        internal string StreamName { get; set; }
        [DataMember]
        internal ImmutableDictionary<long, AccessRights> RelatedUsers { get; set; }
        [DataMember]
        internal int BackStream { get; set; } //TODO: change type

        internal StreamContract(string name, ImmutableDictionary<long, AccessRights> users, int bs)
        {
            StreamName = name;
            RelatedUsers = users;
            BackStream = bs;
        }

    }
}
