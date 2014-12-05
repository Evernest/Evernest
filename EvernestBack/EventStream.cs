using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace EvernestBack
{
    /**
     * EventStream represents an instance of a stream of events and should be matched to a single blob
     * should be created with AzureStorageClient
     */
    class EventStream:IEventStream
    {
        private WriteLocker WriteLock;
        CloudBlockBlob Blob;

        public EventStream( CloudBlockBlob blob, int blobSize)
        {
            this.Blob = blob;
            WriteLock = new WriteLocker(blob, blobSize);
            WriteLock.Store();
        }

        // Push : Give a string, return an ID with the Callback
        public void Push(String message, Action<IAgent> callback)
        {
            WriteLock.Register(message, callback);
        }

        // Pull : Use the ID got when pushing to get back the original string
        public void Pull(UInt64 id, Action<IAgent> callback)
        {
            //ImmutableDictionary<UInt64, UInt64> milestones = writeLock.Milestones;
            //UInt64 firstByte = milestones.LowerBound(id);
            //UInt64 lastByte = milestones.UpperBound(id+1)-1;
            //no "LowerBound" method? really? and it isn't in normal Dictionary even...
            //i miss it, should i reimplement them? making it thread safe may be tough though
            UInt64 firstByte = 42;
            UInt64 lastByte = 1337;
            Byte[] buffer = new Byte[lastByte-firstByte];
            Blob.DownloadRangeToByteArray(buffer, 0, (Int64) firstByte, (Int64) (lastByte - firstByte - 1)); //should probably be DownloadRangeToByteArray
            //then search for the good id in this range
            // TODO...
            Agent r = new Reader(null, id, callback);
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
