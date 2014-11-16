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
    class AzureStorageClient
    {
        private CloudBlobClient blobClient;
        CloudBlobContainer streamContainer;

        public AzureStorageClient()
        {
            // Create the blob client
            CloudStorageAccount storageAccount = null;
            try
            {
                // TODO
                string connectionString = ConfigurationManager.AppSettings.Get(0);
                storageAccount = CloudStorageAccount.Parse(connectionString);
                Console.Read();
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
            streamContainer = blobClient.GetContainerReference("streams");
            streamContainer.CreateIfNotExists();
        }

        public Stream GetStream( String streamStrId )
        {
            CloudBlockBlob blob = streamContainer.GetBlockBlobReference(streamStrId);
            return new Stream( blob );
        }

    }
}
