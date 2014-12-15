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
        private long CurrentId;
        private CloudBlockBlob Blob;

        private LocalCache Cache;

        public EventStream( CloudBlockBlob blob, int bufferSize, uint eventChunkSize)
        {
            CurrentId = 0;
            this.Blob = blob;
            BufferedBlobIO buffer = new BufferedBlobIO(Blob, bufferSize);
            Cache = new LocalCache(buffer, eventChunkSize);
            WriteLock = new WriteLocker(buffer, Cache, CurrentId);
            WriteLock.Store();
        }

        // Push : Give a string, return an ID with the Callback
        public void Push(string message, Action<IAgent> callback)
        {
            WriteLock.Register(message, callback);
        }

        // Pull : Use the ID got when pushing to get back the original string
        public void Pull(long id, Action<IAgent> callback)
        {
            String message;
            if (Cache.FetchEvent(id, out message))
            {
                Agent msgAgent = new Agent(message, id, callback);
                msgAgent.Processed();
            }
        }

        public long Size()
        {
            return WriteLock.CurrentID;
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
