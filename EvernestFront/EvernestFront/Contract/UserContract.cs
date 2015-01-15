using System.Collections.Immutable;
using System.Runtime.Serialization;

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
        internal ImmutableDictionary<string, string> Keys { get; set; } //base64 encoded int
        [DataMember]
        internal ImmutableDictionary<long, AccessRights> RelatedStreams { get; set; }
        [DataMember]
        internal ImmutableDictionary<string, string> OwnedSources { get; set; } //name->key

        internal UserContract(string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<long, AccessRights> strms,
            ImmutableDictionary<string, string> srcs)
        {
            UserName = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            Keys = keys;
            RelatedStreams = strms;
            OwnedSources = srcs;
        }

       
    }
}
