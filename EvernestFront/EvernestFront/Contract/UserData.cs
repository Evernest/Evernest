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
    class UserData
    {
        [DataMember]
        internal readonly string UserName;
        [DataMember]
        internal readonly long UserId;
        [DataMember]
        internal readonly string Key;
        [DataMember]
        internal readonly ImmutableDictionary<long, AccessRights> RelatedStreams;
        [DataMember]
        internal readonly ImmutableDictionary<long, long> OwnedSources;
        // more to come

        internal UserData(string name, long id, string key)
        {
            UserName = name;
            UserId = id;
            Key = key;
            RelatedStreams = ImmutableDictionary<long, AccessRights>.Empty;
            OwnedSources = ImmutableDictionary<long, long>.Empty;
        }

        private UserData(string name, long id, string key, ImmutableDictionary<long, AccessRights> strms,
            ImmutableDictionary<long, long> srcs)
        {
            UserName = name;
            UserId = id;
            Key = key;
            RelatedStreams = strms;
            OwnedSources = srcs;
        }


        internal UserData SetRight(long streamId, AccessRights right)
        {
            var strms = RelatedStreams.Add(streamId, right);
            return new UserData(UserName,UserId,Key,strms,OwnedSources);
        }

    }
}
