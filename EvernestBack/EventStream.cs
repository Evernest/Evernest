using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    /**
     * EventStream represents an instance of a stream of events and should be matched to a single blob
     * should be created with AzureStorageClient
     */
    class EventStream:IEventStream
    {
        private WriteLocker WriteLock;
        private CloudBlockBlob Blob;

        private EventIndexer Indexer;

        public EventStream( CloudPageBlob blob, CloudBlockBlob streamIndexBlob)
        {
            BufferedBlobIO buffer = new BufferedBlobIO(blob);
            Indexer = new EventIndexer(streamIndexBlob, buffer);
            WriteLock = new WriteLocker(buffer, Indexer, 0); // Initial ID = 0
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
            string message;
            if (Indexer.FetchEvent(id, out message))
            {
                Agent msgAgent = new Agent(message, id, callback);
                msgAgent.Processed();
            }
        }

        public long Size()
        {
            return WriteLock.CurrentId;
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
