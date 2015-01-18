using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace EvernestBack
{
    ///AzureStorageClient represents a client connection with the azure storage service 
    ///see it as an EventStream factory
    public class AzureStorageClient
    {
        private CloudBlobClient BlobClient;
        private Dictionary<String, IEventStream> OpenedStreams;
        private CloudBlobContainer StreamContainer;
        private CloudBlobContainer StreamIndexContainer;
        private bool Dummy;

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
                CloudStorageAccount storageAccount;
                try
                {
                    // TODO
                    var connectionString = ConfigurationManager.AppSettings["StorageAccountConnectionString"];
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
                StreamIndexContainer = BlobClient.GetContainerReference("streamindex");
                try
                {
                    StreamContainer.CreateIfNotExists();
                    StreamIndexContainer.CreateIfNotExists();
                }
                catch (StorageException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        ///Get a reference to an already created stream.
        public IEventStream GetEventStream( String streamId ) //not thread-safe yet
        {
            return OpenStream(streamId);
        }

        ///Create a new stream and open it.
        public IEventStream GetNewEventStream(String streamId)
        {
            CreateEventStream(streamId);
            IEventStream stream =  OpenStream(streamId);
            return stream;
        }

        ///Open a stream and throws an exception if it does not exists
        private IEventStream OpenStream(String streamId)
        {
            IEventStream stream;
            if (!OpenedStreams.TryGetValue(streamId, out stream))
            { // If OpenedStreams doesn't contains streamId, there is a problem
                throw new ArgumentException("The stream " + streamId + " does not exists.");
            }
            if(stream != null)
            {
                // If the stream is already opened
                return stream;
            }
            CloudPageBlob streamBlob = StreamContainer.GetPageBlobReference(streamId);
            CloudBlockBlob streamIndexBlob = StreamIndexContainer.GetBlockBlobReference(streamId);
            try
            {
                stream = new EventStream(streamBlob, streamIndexBlob);
                // TODO : Check if EventStream throws an exception if blob references does not exists
            }
            catch(ArgumentNullException) { throw new ArgumentException("You try to open a stream that does not exists"); }
            // Update reference, don't seem to have an "update" method... thank you micro$oft
            OpenedStreams.Remove(streamId);
            OpenedStreams.Add(streamId, stream);
            return stream;
        }

        ///Create a new stream (does not open it). I.e. Create the blob to store it.
        private void CreateEventStream(string streamId)
        {
            if(OpenedStreams.ContainsKey(streamId))
            {
                // If we want to create a stream that already exists
                throw new ArgumentException("You want to create a stream with a name already used.\nStream : " + streamId);
            }
            if(Dummy)
            {
            }
            else
            {
                CloudPageBlob streamBlob = StreamContainer.GetPageBlobReference(streamId);
                streamBlob.Create(PageBlobSize);
                OpenedStreams.Add(streamId, null);
            }
        }

        //missing something to close streams

        public void DeleteIfExists( String streamStringId )
        {
            //TODO
        }

        public bool TryGetFreshEventStream(string streamStringId, out IEventStream stream)
        {
            //TODO
            stream = null;
            return false;
        }
    }
}
