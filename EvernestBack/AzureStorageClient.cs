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
     * AzureStorageClient represents a client connection with the azure storage service 
     * see it as an EventStream factory
     */
    class AzureStorageClient
    {
        private CloudBlobClient blobClient;
        private Dictionary<String, EventStream> openedStreams;
        private CloudBlobContainer streamContainer;
        private int BlobSize;

        public AzureStorageClient()
        {
            openedStreams = new Dictionary<String,EventStream>();
            // Create the blob client
            CloudStorageAccount storageAccount = null;
            try
            {
                // TODO : We should avoid to hardcode the indexes.
                string connectionString = ConfigurationManager.AppSettings.Get(0);
                storageAccount = CloudStorageAccount.Parse(connectionString);
                BlobSize = Int32.Parse(ConfigurationManager.AppSettings.Get(1));
            }
            catch (NullReferenceException e)
            {
                Console.Error.WriteLine("Erreur de configuration du storageAccount");
                Console.Error.WriteLine("Method : {0}", e.TargetSite);
                Console.Error.WriteLine("Message : {0}", e.Message);
                Console.Error.WriteLine("Source : {0}", e.Source);
                Console.Read();
                return;
            }
            blobClient = storageAccount.CreateCloudBlobClient();
            streamContainer = blobClient.GetContainerReference("streams");
            try
            {
                streamContainer.CreateIfNotExists();
            }
            catch (StorageException e)
            {
                Console.Error.WriteLine("Erreur de connection");
                Console.Error.WriteLine("Method : {0}", e.TargetSite);
                Console.Error.WriteLine("Message : {0}", e.Message);
                Console.Error.WriteLine("Source : {0}", e.Source);
                Console.Read();
                return;
            }
        }


        public EventStream GetEventStream( String streamStrId )
        {
            EventStream stream;
            if( !openedStreams.TryGetValue(streamStrId, out stream) )
            {
                stream = new EventStream(streamContainer.GetBlockBlobReference(streamStrId), BlobSize);
                openedStreams.Add(streamStrId, stream);
            }
            return stream;
        }
        //missing something to close streams

    }
}
