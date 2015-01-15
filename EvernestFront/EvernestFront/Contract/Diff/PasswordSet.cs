using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class PasswordSet : IDiff
    {
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal string SaltedPasswordHash { get; set; }

        internal PasswordSet(long userId, string saltedPasswordHash)
        {
            UserId = userId;
            SaltedPasswordHash = saltedPasswordHash;
        }
    }
}
