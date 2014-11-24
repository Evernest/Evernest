using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    internal class UserAdded : IDiff
    {
        [DataMember]
        internal readonly string UserName;
        [DataMember]
        internal readonly long UserId;
        [DataMember]
        internal readonly string Key;
    }
}
