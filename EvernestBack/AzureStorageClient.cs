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
    /*
     * AzureStorageClient represents a client connection with the azure storage service 
     * see it as an EventStream factory
     */
    class AzureStorageClient
    {
        private CloudBlobClient blobClient;
        private Dictionary<String, EventStream> openedStreams;
        private CloudBlobContainer streamContainer;

        public AzureStorageClient()
        {
            openedStreams = new Dictionary<String,EventStream>();
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


        public EventStream GetEventStream( String streamStrId )
        {
            EventStream stream;
            if( !openedStreams.TryGetValue(streamStrId, out stream) )
            {
                stream = new EventStream(streamContainer.GetBlockBlobReference(streamStrId));
                openedStreams.Add(streamStrId, stream);
            }
            return stream;
        }
        //missing something to close streams

    }
}
