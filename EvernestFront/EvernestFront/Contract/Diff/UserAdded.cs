using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    internal class UserAdded : IDiff
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
