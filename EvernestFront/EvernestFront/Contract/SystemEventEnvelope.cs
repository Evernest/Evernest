using System.Runtime.Serialization;
using EvernestFront.Utilities;
using EvernestFront.Contract.SystemEvents;
using Newtonsoft.Json;

namespace EvernestFront.Contract
{
    [DataContract]
    class SystemEventEnvelope
    {
        [DataMember]
        public string SystemEventType;
        
        [DataMember]
        public string SerializedSystemEvent;

        [JsonConstructor]
        public SystemEventEnvelope(string systemEventType, string serializedSystemEvent)
        {
            SystemEventType = systemEventType;
            SerializedSystemEvent = serializedSystemEvent;
        }

        public SystemEventEnvelope(ISystemEvent systemEvent)
        {
            var serializer = new Serializer();
            SystemEventType = (systemEvent.GetType()).Name;
            SerializedSystemEvent = serializer.WriteContract(systemEvent);
        }
    }
}
