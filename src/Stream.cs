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


        class StreamController
        {
        }

        class StreamDeliver
        {
            protected void DeliverFeedBack(Producer prod)
            {
                // TODO - Complete with feedback in frontend
                Console.WriteLine("La requete {0} a retourné {1}", prod.GetRequestID(), prod.GetMessage().ToString());
            }
        }
    }
}
