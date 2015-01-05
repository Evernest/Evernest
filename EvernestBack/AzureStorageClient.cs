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
                    Console.Error.WriteLine("StorageAccount configuration error:");
                    Console.Error.WriteLine("Method : {0}", e.TargetSite);
                    Console.Error.WriteLine("Message : {0}", e.Message);
                    Console.Error.WriteLine("Source : {0}", e.Source);
                    return;
                }
                BlobClient = storageAccount.CreateCloudBlobClient();
                StreamContainer = BlobClient.GetContainerReference("stream");
                StreamIndexContainer = BlobClient.GetContainerReference("streamIndex");
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


        /**
         * Get a reference to an already created stream.
         */
        public IEventStream GetEventStream( String streamID ) //not thread-safe yet
        {
            IEventStream stream = OpenStream(streamID);
            return stream;
        }


        /**
         * Create a new stream and open it.
         */
        public IEventStream GetNewEventStream(String streamID)
        {
            CreateEventStream(streamID);
            IEventStream stream =  OpenStream(streamID);
            return stream;
        }

        /**
         * Open a stream and throws an exception if it does not exists
         * 
         */
        private IEventStream OpenStream(String streamID)

        {
            IEventStream stream;
            if (!OpenedStreams.TryGetValue(streamID, out stream))
            { // If OpenedStreams doesn't contains streamID, there is a problem
                throw new ArgumentException("The stream " + streamID + " does not exists.");
            }
            if(stream != null)
            {
                // If the stream is already opened
                return stream;
            }
            CloudPageBlob streamBlob = StreamContainer.GetPageBlobReference(streamID);
            CloudBlockBlob streamIndexBlob = StreamIndexContainer.GetBlockBlobReference(streamID);
            try
            {
                stream = new EventStream(streamBlob, streamIndexBlob, BufferSize, EventChunkSize);
                // TODO : Check if EventStream throws an exception if blob references does not exists
            }
            catch(ArgumentNullException e) { throw new ArgumentException("You try to open a stream that does not exists"); }
            // Update reference, don't seem to have an "update" method... thank you micro$oft
            OpenedStreams.Remove(streamID);
            OpenedStreams.Add(streamID, stream);
            return stream;
        }

        /**
         * Create a new stream (does not open it). I.e. Create the blob to store it.
         */
        private void CreateEventStream(String streamID)

        {
            if(OpenedStreams.ContainsKey(streamID))
            {
                // If we want to create a stream that already exists
                throw new ArgumentException("You want to create a stream with a name already used.\nStream : " + streamID);
            }
            if(Dummy)
            {
            } else {
                CloudPageBlob streamBlob = StreamContainer.GetPageBlobReference(streamID);
                CloudBlockBlob streamIndexBlob = StreamIndexContainer.GetBlockBlobReference(streamID);
                streamBlob.Create(PageBlobSize);
                // TODO : Create streamIndexBlob ?
                OpenedStreams.Add(streamID, null);
            }
        }

        //missing something to close streams

    }
}
