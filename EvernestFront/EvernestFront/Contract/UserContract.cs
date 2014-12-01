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
        internal ImmutableDictionary<string, string> OwnedSources { get; set; } //name->key

        internal UserContract(string name, string sph, byte[] ps, string key, ImmutableDictionary<long, AccessRights> strms,
            ImmutableDictionary<string, string> srcs)
        {
            UserName = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            Key = key;
            RelatedStreams = strms;
            OwnedSources = srcs;
        }

       
    }
}
