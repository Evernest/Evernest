using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace Cloud14
{
    class Stream
    {
        private CloudBlobClient blobClient;
        private WriteLocker writeLock;

        public Stream()
        {
            // Create the blob client
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            blobClient = storageAccount.CreateCloudBlobClient();

            writeLock = new WriteLocker(blobClient);
        }


        public void StreamWrite(String message, Int64 id)
        {
            Message m = new Message(message, id);
            Producer p = new Producer(m, id, writeLock, this);
        }

        public void StreamRead()
        {

        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.GetMessage().ToString());
        }
    }
}
