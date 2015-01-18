using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class UserDeletedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal long UserId { get; set; }

        internal UserDeletedSystemEvent(string name, long userId)
        {
            UserName = name;
            UserId = userId;
        }
    }
}
