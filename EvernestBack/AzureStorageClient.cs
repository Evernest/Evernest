using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
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
        private CloudBlobContainer StreamIndexContainer;
        private bool Dummy;
        private int BufferSize;

        private uint EventChunkSize;
        private static AzureStorageClient _singleton;

        public static AzureStorageClient Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new AzureStorageClient();
                return _singleton;   
            }
        }

        private long PageBlobSize;
        
        private AzureStorageClient()
        {
            Dummy = Boolean.Parse(ConfigurationManager.AppSettings["Dummy"]);
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
                StreamContainer = BlobClient.GetContainerReference("stream");
                StreamIndexContainer = BlobClient.GetContainerReference("streamindex");
                try
                {
                    StreamContainer.CreateIfNotExists();
                    StreamIndexContainer.CreateIfNotExists();
                }
                catch (StorageException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

        public IEventStream GetEventStream( string streamStringID ) //not thread-safe yet
        {
            IEventStream stream;
            if( !OpenedStreams.TryGetValue(streamStringID, out stream) )
            {
                throw new ArgumentException("You want to recover a stream that does not exists.\n Stream : " + streamStringID);
            }
            return stream;
        }

        public IEventStream GetNewEventStream(string streamStringID)
        {
            if(OpenedStreams.ContainsKey(streamStringID))
            {
                // If we want to create a stream that already exists 
                throw new ArgumentException("You want to create a stream with a name already used.\n Stream : " + streamStringID);
            }
            IEventStream stream;
            if(Dummy)
            {
                Console.WriteLine("Starting RAMStream.");
                stream = new RAMStream(streamStringID);
            } else {
                CloudPageBlob streamBlob = StreamContainer.GetPageBlobReference(streamStringID);
                CloudBlockBlob streamIndexBlob = StreamIndexContainer.GetBlockBlobReference(streamStringID);
                streamBlob.Create(PageBlobSize);
                stream = new EventStream(streamBlob, streamIndexBlob, BufferSize, EventChunkSize);
            }
            OpenedStreams.Add(streamStringID, stream);
            return stream;

        }
        //missing something to close streams

        public void DeleteIfExists( String streamStringID )
        {
            //TODO
        }

        public bool TryGetFreshEventStream(string streamStringID, out IEventStream stream)
        {
            //TODO
            stream = null;
            return false;
        }
    }
}
