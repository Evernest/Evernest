using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class DiffEnvelope
    {
        [DataMember]
        public string DiffType;
        
        [DataMember]
        public string SerializedDiff;

        public DiffEnvelope(IDiff diff)
        {
            DiffType = (diff.GetType()).Name;
            SerializedDiff = Serializing.WriteContract(diff);
        }
    }
}
