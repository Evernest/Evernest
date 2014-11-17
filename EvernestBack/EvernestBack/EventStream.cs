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
    /* EventStream represents an instance of a stream of events and should be matched to a single blob
     * should be created with AzureStorageClient
     */
    class EventStream
    {
        private WriteLocker writeLock;
        private UInt64 currentId;

        public EventStream( CloudBlockBlob blob )
        {
            writeLock = new WriteLocker(blob);
            currentId = 0;
        }

        public UInt64 Push(String message)
        {
            UInt64 tmp = currentId;
            Agent p = new Producer(message, currentId, writeLock, this);
            currentId++;
            return tmp;
        }

        public void Pull(UInt64 id)
        {
            Agent r = new Reader(null, id, this);
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
