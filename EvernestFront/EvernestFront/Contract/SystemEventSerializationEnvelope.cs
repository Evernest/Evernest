using System.Runtime.Serialization;
using EvernestFront.Auxiliaries;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Contract
{
    [DataContract]
    class SystemEventSerializationEnvelope
    {
        [DataMember]
        public string SystemEventType;
        
        [DataMember]
        public string SerializedSystemEvent;

        public SystemEventSerializationEnvelope(ISystemEvent systemEvent)
        {
            SystemEventType = (systemEvent.GetType()).Name;
            SerializedSystemEvent = Serializer.WriteContract(systemEvent);
        }
    }
}
