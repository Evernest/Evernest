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
        public static AzureStorageClient singleton = new AzureStorageClient();
        
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
                    BufferSize = Int32.Parse(ConfigurationManager.AppSettings["BufferSize"]);
                    EventChunkSize = UInt32.Parse(ConfigurationManager.AppSettings["EventChunkSize"]);
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
                throw new ArgumentException("You want to recover a stream that does not exists.\n Stream : " + streamStringID);
            }
            return stream;
        }

        public IEventStream GetNewEventStream(String streamStringID)
        {
            if(OpenedStreams.ContainsKey(streamStringID))
            {
                // If we want to create a stream that already exists 
                throw new ArgumentException("You want to create a stream with a name already used.\n Stream : " + streamStringID);
            }
            IEventStream stream;
            if(Dummy)
            {
                stream = new RAMStream(streamStringID);
            } else {
                stream = new EventStream(StreamContainer.GetBlockBlobReference(streamStringID), BufferSize, EventChunkSize);
            }
            OpenedStreams.Add(streamStringID, stream);
            return stream;

        }
        //missing something to close streams

    }
}
