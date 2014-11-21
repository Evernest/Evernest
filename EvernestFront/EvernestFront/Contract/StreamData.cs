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
    class StreamData
    {
        [DataMember]
        internal readonly string StreamName;
        [DataMember]
        internal readonly long StreamId;
        [DataMember]
        internal readonly ImmutableDictionary<long, AccessRights> RelatedUsers;
        // more to come

        internal StreamData(string name, long id)
        {
            StreamName = name;
            StreamId = id;
            RelatedUsers = ImmutableDictionary<long, AccessRights>.Empty;
        }

        private StreamData(string name, long id, ImmutableDictionary<long, AccessRights> users)
        {
            StreamName = name;
            StreamId = id;
            RelatedUsers = users;
        }

        internal StreamData SetRight(long userId, AccessRights right)
        {
            var users = RelatedUsers.Add(userId, right);
            return new StreamData(StreamName, StreamId, users);
        }
    }
}
