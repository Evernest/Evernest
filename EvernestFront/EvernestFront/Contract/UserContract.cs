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
    class UserContract
    {
        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal string SaltedPasswordHash { get; set; }
        [DataMember]
        internal byte[] PasswordSalt { get; set; }
        [DataMember]
        internal string Key { get; set; } //base64 encoded int
        [DataMember]
        internal ImmutableDictionary<long, AccessRights> RelatedStreams { get; set; }
        [DataMember]
        internal ImmutableDictionary<long, long> OwnedSources { get; set; }

        internal UserContract(string name, string sph, byte[] ps, string key, ImmutableDictionary<long, AccessRights> strms,
            ImmutableDictionary<long, long> srcs)
        {
            UserName = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            Key = key;
            RelatedStreams = strms;
            OwnedSources = srcs;
        }

        //[DataMember]
        //internal readonly string UserName;
        //[DataMember]
        //internal readonly long UserId;
        //[DataMember]
        //internal readonly string Key;
        //[DataMember]
        //internal readonly ImmutableDictionary<long, AccessRights> RelatedStreams;
        //[DataMember]
        //internal readonly ImmutableDictionary<long, long> OwnedSources;
        // more to come

        //internal UserData(string name, long id, string key)
        //{
        //    UserName = name;
        //    UserId = id;
        //    Key = key;
        //    RelatedStreams = ImmutableDictionary<long, AccessRights>.Empty;
        //    OwnedSources = ImmutableDictionary<long, long>.Empty;
        //}

        //private UserData(string name, long id, string key, ImmutableDictionary<long, AccessRights> strms,
        //    ImmutableDictionary<long, long> srcs)
        //{
        //    UserName = name;
        //    UserId = id;
        //    Key = key;
        //    RelatedStreams = strms;
        //    OwnedSources = srcs;
        //}


        //internal UserContract SetRight(long streamId, AccessRights right)
        //{
        //    var strms = RelatedStreams.Add(streamId, right);
        //    return new UserContract(UserName,UserId,Key,strms,OwnedSources);
        //}

    }
}
