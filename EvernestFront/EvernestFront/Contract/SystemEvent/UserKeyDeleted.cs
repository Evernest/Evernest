using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class UserKeyDeleted : ISystemEvent
    {
        [DataMember]
        internal string Key { get; set; } //base64 encoded int
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal string KeyName { get; set; }

        internal UserKeyDeleted(string key, long userId, string name)
        {
            Key = key;
            UserId = userId;
            KeyName = name;
        }
    }
}
