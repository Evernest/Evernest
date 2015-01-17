using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class SourceCreated : ISystemEvent
    {
        [DataMember]
        internal string SourceKey { get; private set; }
        [DataMember]
        internal string SourceName { get; private set; }
        [DataMember]
        internal long UserId { get; private set; }
        [DataMember]
        internal string UserName { get; private set; }
        [DataMember]
        internal long EventStreamId { get; private set; }
        [DataMember]
        internal AccessRight Right { get; private set; }

        
        internal SourceCreated(string key, string name, long userId, string userName, long eventStreamId, AccessRight right)
        {
            SourceKey = key;
            SourceName = name;
            UserId = userId;
            UserName = userName;
            EventStreamId = eventStreamId;
            Right = right;
        }
    }
}
