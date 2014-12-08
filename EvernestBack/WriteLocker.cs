using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;


namespace EvernestBack
{
    class WriteLocker
	{
        private class PendingEvent
        {
            public String Message { get; private set; }
            public Action<IAgent> Callback { get; private set; }
            public PendingEvent(String message, Action<IAgent> callback)
            {
                Message = message;
                Callback = callback;
            }
        }
		private BlockingCollection<PendingEvent> PendingEventCollection = new BlockingCollection<PendingEvent>();
        private CloudBlobStream Output;
        private CloudBlockBlob Blob;
        public History<UInt64> Milestones { get; protected set;}
        public UInt64 TotalWrittenBytes { get; protected set;}
            //i'm not really satisfied with that, feel free to move it wherever you think it more appropriate
        private UInt64 CurrentID = 0;
        
        public WriteLocker(CloudBlockBlob blob, int blobSize)
        {
            TotalWrittenBytes = 0;
            this.Blob = blob;
            blob.StreamWriteSizeInBytes = blobSize; // Defined in app.config
            Milestones = new History<UInt64>();
            Output = blob.OpenWrite();
        }

        public void Store(UInt32 eventChunkSizeInBytes = 16384) //16KiB for now, also arbitrary
        {
            Task.Run(() =>
            {
                UInt16 wroteBytes;
                UInt32 chunkBytes = 0;
                UInt64 lastPosition = 0;
                //Console.WriteLine("Starting Storing"); //if there is a Console.Read() in the main thread, this will block this instruction
                while (PendingEventCollection.Count > 0)
                {
                    PendingEvent pendingEvent = PendingEventCollection.Take();
                    Agent agent = new Agent(pendingEvent.Message, CurrentID, pendingEvent.Callback);
                    CurrentID++;
                    wroteBytes = StoreToCloud(agent);
                    TotalWrittenBytes += wroteBytes;
                    if( wroteBytes + chunkBytes > eventChunkSizeInBytes)
                    {
                        lastPosition += chunkBytes;
                        Milestones.Insert(agent.RequestID, lastPosition);
                        chunkBytes = wroteBytes;
                    }
                    else
                        chunkBytes += wroteBytes;
                    agent.Processed();
                }
            }
            );
        }

        private UInt16 StoreToCloud(Agent prod)
        {
            UInt16 size;
            Byte[] bytes = prod.Serialize(out size);
            //Output.Write(bytes, 0, size);
            Blob.UploadFromByteArray(bytes, 0, (int) size);
            return size;
        }

        public void Register(String message, Action<IAgent> callback)
        {
            PendingEventCollection.Add(new PendingEvent(message, callback));
        }
	}
}
