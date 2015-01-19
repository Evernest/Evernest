using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class UserDeletedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal long UserId { get; set; }

        [JsonConstructor]
        internal UserDeletedSystemEvent(string name, long userId)
        {
            UserName = name;
            UserId = userId;
        }
    }
}
