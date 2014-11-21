using System.Runtime.Serialization;

namespace EvernestFront.Contract.DataModified
{
    [DataContract]
    internal class UserAdded : IDataModified
    {
        [DataMember]
        internal readonly string UserName;
        [DataMember]
        internal readonly long UserId;
        [DataMember]
        internal readonly string Key; //?
    }
}
