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
        private UInt64 pushReqID;
        private UInt64 pullReqID;

        public EventStream( CloudBlockBlob blob )
        {
            writeLock = new WriteLocker(blob);
            pushReqID = pullReqID = 0;
        }

        public UInt64 Push(String message)
        {
            UInt64 tmp = pushReqID;
            Producer.Create(message, pushReqID, writeLock, this);
            pushReqID++;
            return tmp;
        }

        public UInt64 Pull()
        {
            UInt64 tmp = pullReqID;
            Agent r = new Reader(null, pullReqID, this);
            pullReqID++;
            return tmp;
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
