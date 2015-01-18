using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    internal class UserCreatedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal string SaltedPasswordHash { get; set; }
        [DataMember]
        internal byte[] PasswordSalt { get; set; }

        internal UserCreatedSystemEvent(string name, long userId, string sph, byte[] ps)
        {
            UserName = name;
            UserId = userId;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
        }
    }
}
