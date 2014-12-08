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
        private Dictionary<String, IEventStream> openedStreams;
        private CloudBlobContainer streamContainer;
        private bool dummy;
        private int blobSize;

        public AzureStorageClient(bool dummy = true)
        {
            this.dummy = dummy;
            openedStreams = new Dictionary<String, IEventStream>();
            if (dummy) // temporary dummy mode
            {
                blobClient = null;
                streamContainer = null;
            }
            else
            {
                // Create the blob client
                CloudStorageAccount storageAccount = null;
                try
                {
                    // TODO
                    var connectionString = ConfigurationManager.AppSettings["StorageAccountConnectionString"];
                    blobSize = Int32.Parse(ConfigurationManager.AppSettings["BlobSize"]);
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
                try
                {
                    streamContainer.CreateIfNotExists();
                }
                catch (StorageException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }


        public IEventStream GetEventStream( String streamStringID ) //not thread-safe yet
        {
            IEventStream stream;
            if( !openedStreams.TryGetValue(streamStringID, out stream) )
            {
                //should ensure that BlockSearchMode is set to Latest
                if (dummy)
                    stream = new RAMStream(streamStringID);
                else
                    stream = new EventStream(streamContainer.GetBlockBlobReference(streamStringID), blobSize);
                openedStreams.Add(streamStringID, stream);
            }
            return stream;
        }
        //missing something to close streams

    }
}
