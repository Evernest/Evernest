using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EvernestFront.Contract;

namespace EvernestFront
{
    static class Serializing
    {
        internal static string WriteContract<T>(T contract)
        {
            var dcs = new DataContractSerializer(typeof(T));
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                dcs.WriteObject(writer, contract);
                writer.Flush();
                return sb.ToString();
            }
        }


        internal static T ReadContract<T>(String serializedContract)
        {
            var dcs = new DataContractSerializer(typeof (T));
            var sr = new StringReader(serializedContract);
            using (var reader = XmlReader.Create(sr))
            {
                return (T) dcs.ReadObject(reader);
            }
        }
    }
}
