using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class UserDeleted : ISystemEvent
    {
        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal long UserId { get; set; }

        internal UserDeleted(string name, long userId)
        {
            UserName = name;
            UserId = userId;
        }
    }
}
