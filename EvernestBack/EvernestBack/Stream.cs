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
    class Stream
    {
        private CloudBlobClient blobClient;
        private WriteLocker writeLock;

        public Stream()
        {
            // Create the blob client
            CloudStorageAccount storageAccount = null;
            try
            {
                // TODO
                string connectionString = ConfigurationManager.AppSettings.Get(0);
                storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (NullReferenceException e)
            {
                Console.Error.WriteLine("Erreur de configuration du storageAccount");
                Console.Error.WriteLine("Method : {0}", e.TargetSite);
                Console.Error.WriteLine("Message : {0}", e.Message);
                Console.Error.WriteLine("Source : {0}", e.Source);
                return;
            }
            blobClient = storageAccount.CreateCloudBlobClient();

            writeLock = new WriteLocker(blobClient);
        }


        public void Push(String message, Int64 id)
        {
            Agent p = new Producer(message, id, writeLock, this);
        }

        public void Pull(Int64 id)
        {
            Agent r = new Reader(null, id, this);
        }

        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.message);
        }
    }
}
