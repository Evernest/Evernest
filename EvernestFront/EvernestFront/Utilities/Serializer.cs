using System;
using System.Collections.Generic;
using System.Linq;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using Newtonsoft.Json;

namespace EvernestFront.Utilities
{
    public static class AllContracts
    {
        public static Dictionary<string, Type> GetTypeMapForEvents()
        {
            return typeof(AllContracts).Assembly.GetTypes()
                .Where(t => typeof(ISystemEvent).IsAssignableFrom(t))
                .ToDictionary(t => t.Name, t => t);
        }
    }

    class Serializer
    {
        internal string WriteContract<T>(T contract)
        {
            return JsonConvert.SerializeObject(contract);
        }


        internal T ReadContract<T>(string serializedContract)
        {
            return JsonConvert.DeserializeObject<T>(serializedContract);
        }

        internal ISystemEvent ReadDiffEnvelope(string serializedEnvelope)
        {
            SystemEventEnvelope envelope = ReadContract<SystemEventEnvelope>(serializedEnvelope);
            string typeName = envelope.SystemEventType;
            var map = AllContracts.GetTypeMapForEvents();
            var type = map[typeName];
            return
                (ISystemEvent)
                    typeof (string).GetMethod("JsonConvert.DeserializeObject")
                        .MakeGenericMethod(type)
                        .Invoke(null, new object[] {envelope.SerializedSystemEvent});



        }
    }
}
