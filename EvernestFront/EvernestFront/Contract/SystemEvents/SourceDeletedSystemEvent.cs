using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class SourceDeletedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal string SourceKey { get; set; }
        [DataMember]
        internal string SourceName { get; set; }
        [DataMember]
        internal long SourceId { get; set; }
        [DataMember]
        internal long UserId { get; set; }

        internal SourceDeletedSystemEvent(string key, string name, long id, long userId)
        {
            SourceKey = key;
            SourceName = name;
            SourceId = id;
            UserId = userId;
        }
    }
}
