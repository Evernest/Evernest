using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract
{
    /// <summary>
    /// This contract allows to serialize/deserialize all system events in a polymorphic manner (see Utilities.Serializer.ReadSystemEventEnvelope)
    /// </summary>
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
    }
}
