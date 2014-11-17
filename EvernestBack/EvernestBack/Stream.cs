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
    class Stream:IStream
    {
        private CloudBlockBlob blob;
        private WriteLocker writeLock;
        private UInt64 currentId;

        public Stream( CloudBlockBlob blob )
        {
            writeLock = new WriteLocker(blob);
            currentId = 0;
        }

        public UInt64 Push(String message, Action<IAgent> Callback)
        {
            UInt64 tmp = currentId;
            Agent p = new Producer(message, currentId, writeLock, Callback);
            currentId++;
            return tmp;
        }

        public void Pull(UInt64 id, Action<IAgent> Callback)
        {
            Agent r = new Reader(null, id, Callback);
        }
    }
}
