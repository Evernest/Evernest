using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace EvernestFront.Contract
{
    [DataContract]
    class EventStreamContract
    {
        [DataMember]
        internal string StreamName { get; set; }
        [DataMember]
        internal ImmutableDictionary<long, AccessRights> RelatedUsers { get; set; }
        //[DataMember]
        internal EvernestBack.IEventStream BackStream { get; set; }
        //this is a temporary simulator of backend
        //the actual class should have a contract

        internal EventStreamContract(string name, ImmutableDictionary<long, AccessRights> users, EvernestBack.IEventStream bs)
        {
            StreamName = name;
            RelatedUsers = users;
            BackStream = bs;
        }

    }
}
