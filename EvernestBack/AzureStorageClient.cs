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
    public class AzureStorageClient
    {
        private CloudBlobClient BlobClient;
        private Dictionary<String, IEventStream> OpenedStreams;
        private CloudBlobContainer StreamContainer;
        private bool Dummy;
        private int BufferSize;
        private UInt32 EventChunkSize;
        private long PageBlobSize;
        
        public AzureStorageClient(bool dummy = true)
        {
            Dummy = dummy;
            OpenedStreams = new Dictionary<String, IEventStream>();
            if (Dummy) // temporary dummy mode
            {
                BlobClient = null;
                StreamContainer = null;
            }
            else
            {
                // Create the blob client
                CloudStorageAccount storageAccount = null;
                try
                {
                    // TODO
                    var connectionString = ConfigurationManager.AppSettings["StorageAccountConnectionString"];
                    BufferSize = Int32.Parse(ConfigurationManager.AppSettings["MinimumBufferSize"]);
                    EventChunkSize = UInt32.Parse(ConfigurationManager.AppSettings["EventChunkSize"]);
                    PageBlobSize = UInt32.Parse(ConfigurationManager.AppSettings["PageBlobSize"]);
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
                BlobClient = storageAccount.CreateCloudBlobClient();
                StreamContainer = BlobClient.GetContainerReference("streams");
                try
                {
                    StreamContainer.CreateIfNotExists();
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
            if( !OpenedStreams.TryGetValue(streamStringID, out stream) )
            {
                //should ensure that BlockSearchMode is set to Latest
                if (Dummy)
                    stream = new RAMStream(streamStringID);
                else
                {
                    CloudPageBlob pageBlob = StreamContainer.GetPageBlobReference(streamStringID);
                    pageBlob.Create(PageBlobSize);
                    stream = new EventStream(pageBlob, BufferSize, EventChunkSize);
                }
                OpenedStreams.Add(streamStringID, stream);
            }
            return stream;
        }
        //missing something to close streams

    }
}
