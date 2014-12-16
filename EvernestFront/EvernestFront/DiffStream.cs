using System;
using System.Runtime.Remoting.Messaging;
using EvernestBack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Answers;
using EvernestFront.Contract.Diff;

namespace EvernestFront
{
    static class DiffStream
    {

        //public long Count { get { return (int)BackStream.Index; } }
        //public long LastEventId { get { return Count - 1; } }


        internal static AzureStorageClient asc = AzureStorageClient.singleton;  //TODO : change this when AzureStorageClient becomes a singleton
        private static EvernestBack.IEventStream BackStream { get; set; }

        private const string DiffstreamStringId = "DiffStream"; //other stream IDs are made of digits, so there is no conflict. Still, probably not a good thing to hardcode this...

        static DiffStream()
        {
            BackStream = asc.GetEventStream(DiffstreamStringId);
        }

        internal static void Push(IDiff diff)
        {
            var contract = new DiffEnvelope(diff);
            BackStream.Push(Serializing.WriteContract(contract), (a => { }));   //change callback to wake up projection ?
        }

        internal static IDiff Pull(long id)
        {
            //TODO : check ID validity
            IDiff diff = null;
            BackStream.Pull(id, (a => diff = Serializing.ReadDiffEnvelope(a.Message)));
            return diff;
        }
    }
}
