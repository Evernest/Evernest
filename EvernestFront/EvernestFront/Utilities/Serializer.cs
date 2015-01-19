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

        internal ISystemEvent ReadSystemEventEnvelope(string serializedEnvelope)
        {
            SystemEventEnvelope envelope = ReadContract<SystemEventEnvelope>(serializedEnvelope);
            string typeName = envelope.SystemEventType;
            var map = AllContracts.GetTypeMapForEvents();
            var type = map[typeName];
            var methodInfo =
                typeof (Newtonsoft.Json.JsonConvert).GetMethods().First(method => method.Name == "DeserializeObject" && method.IsGenericMethod &&
                                                                                  method.GetParameters().Length == 1);
            var generic = methodInfo.MakeGenericMethod(type);
            return (ISystemEvent) generic.Invoke(null, new object[] {envelope.SerializedSystemEvent});



        }
    }
}
