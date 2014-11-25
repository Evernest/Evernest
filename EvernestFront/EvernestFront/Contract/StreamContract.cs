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
        //[DataMember]
        internal EvernestBack.RAMStream BackStream { get; set; }
        //this is a temporary simulator of backend
        //the actual class should have a contract

        internal StreamContract(string name, ImmutableDictionary<long, AccessRights> users, EvernestBack.RAMStream bs)
        {
            StreamName = name;
            RelatedUsers = users;
            BackStream = bs;
        }

    }
}
