using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class SourceCreated : IDiff
    {
        [DataMember]
        internal string Key { get; set; } //base64 encoded int
        [DataMember]
        internal SourceContract SourceContract { get; set; }

        internal SourceCreated(string key, SourceContract sc)
        {
            Key = key;
            SourceContract = sc;
        }
    }
}
