using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class PasswordSet : ISystemEvent
    {
        [DataMember]
        internal long UserId { get; private set; }
        [DataMember]
        internal string SaltedPasswordHash { get; private set; }
        [DataMember]
        internal byte[] PasswordSalt { get; private set; }

        internal PasswordSet(long userId, string hash, byte[] salt)
        {
            UserId = userId;
            SaltedPasswordHash = hash;
            PasswordSalt = salt;
        }
    }
}
