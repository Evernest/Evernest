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
        private LocalCache Cache;

        public EventStream( CloudBlockBlob blob, int bufferSize, UInt32 eventChunkSize)
        {
            BufferedBlobIO buffer = new BufferedBlobIO(blob, bufferSize);
            Cache = new LocalCache(buffer, eventChunkSize);
            WriteLock = new WriteLocker(buffer, Cache);
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
            String message;
            if (Cache.FetchEvent(id, out message))
            {
                Agent msgAgent = new Agent(message, id, callback);
                msgAgent.Processed();
            }
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
