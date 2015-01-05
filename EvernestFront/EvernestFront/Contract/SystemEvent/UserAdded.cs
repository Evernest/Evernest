using System.Runtime.Serialization;
using EvernestFront.Errors;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    internal class UserAdded : ISystemEvent
    {
        
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal UserContract UserContract { get; set; }

        internal UserAdded(long userId, UserContract userContract)
        {
            UserId = userId;
            UserContract = userContract;
        }
    }
}
