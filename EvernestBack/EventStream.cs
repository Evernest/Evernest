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
        private UInt64 CurrentId;
        private CloudBlockBlob Blob;

        public EventStream(CloudBlockBlob blob, int blobSize)
        {
            CurrentId = 0;
            this.Blob = blob;
            WriteLock = new WriteLocker(Blob, blobSize, CurrentId);
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
            History<UInt64> milestones = WriteLock.Milestones;
            UInt64 firstByte = 0;
            UInt64 lastByte = 0;
            if (!milestones.UpperBound(id + 1, ref lastByte) && (lastByte = WriteLock.TotalWrittenBytes) == 0)
                return; //there's nothing written!
            Byte[] buffer = new Byte[lastByte-firstByte];
            Blob.DownloadRangeToByteArray(buffer, 0, (Int64) firstByte, (Int64) (lastByte-firstByte));
            UInt32 currentPosition = 0, messageLength = 0;
            UInt64 currentID = 0;
            do
            {
                currentPosition += messageLength;
                if(!BitConverter.IsLittleEndian)
                {
                    Agent.Reverse(buffer, (int) currentPosition, (int) sizeof(UInt64));
                    Agent.Reverse(buffer, (int) currentPosition+sizeof(UInt64), (int) sizeof(UInt16));
                }
                currentID = BitConverter.ToUInt64(buffer, (int) currentPosition);
                messageLength = BitConverter.ToUInt16(buffer, (int) currentPosition+sizeof(UInt64));
                currentPosition += sizeof(UInt64)+sizeof(UInt16);
            }
            while(currentID != id); // /!\ should also check whether the position is still in range!
            String message = System.Text.Encoding.Unicode.GetString(buffer, (int) currentPosition, (int) messageLength);
            Agent msgAgent = new Agent(message, currentID, callback);
            msgAgent.Processed();
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
