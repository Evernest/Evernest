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
    class EventStream:IStream
    {
        private WriteLocker writeLock;
        private UInt64 currentId;

        public EventStream( CloudBlockBlob blob )
        {
            writeLock = new WriteLocker(blob);
            currentId = 0;
        }


        // Push : Give a string, return an ID with the Callback
        public void Push(String message, Action<IAgent> Callback)
        {
            Agent p = new Producer(message, currentId, writeLock, Callback);
            currentId++;
        }


        // Pull : Use the ID got when pushing to get back the original string
        public void Pull(UInt64 id, Action<IAgent> Callback)
        {
            Agent r = new Reader(null, id, Callback);
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
