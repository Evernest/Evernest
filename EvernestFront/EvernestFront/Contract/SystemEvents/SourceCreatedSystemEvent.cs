using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class SourceCreatedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal string SourceKey { get; private set; }
        [DataMember]
        internal string SourceName { get; private set; }
        [DataMember]
        internal long SourceId { get; private set; }
        [DataMember]
        internal long UserId { get; private set; }

        
        internal SourceCreatedSystemEvent(string key, string name, long id, long userId)
        {
            SourceKey = key;
            SourceName = name;
            SourceId = id;
            UserId = userId;
        }
    }
}
