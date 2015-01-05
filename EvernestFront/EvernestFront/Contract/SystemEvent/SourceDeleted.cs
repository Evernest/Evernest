using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class SourceDeleted : ISystemEvent
    {
        [DataMember]
        internal string SourceKey { get; set; } //base64 encoded int
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal string SourceName { get; set; }

        internal SourceDeleted(string key, long userId, string name)
        {
            SourceKey = key;
            UserId = userId;
            SourceName = name;
        }
    }
}
