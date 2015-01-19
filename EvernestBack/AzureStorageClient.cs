using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    /// <summary>AzureStorageClient represents a client connection with the azure storage service 
    /// see it as an EventStream factory</summary>
    public class AzureStorageClient
    {
        private static readonly object _initializeLock = new object();
        private static AzureStorageClient _singleton;
        private readonly CloudBlobClient _blobClient;
        private readonly bool _dummy;
        private readonly Dictionary<String, IEventStream> _openedStreams;

        internal readonly long PageBlobSize;
        internal readonly string DummyDataPath;
        internal readonly CloudBlobContainer StreamContainer;
        internal readonly CloudBlobContainer StreamIndexContainer;

        private AzureStorageClient()
        {
            _dummy = Boolean.Parse(ConfigurationManager.AppSettings["Dummy"]);
            _openedStreams = new Dictionary<String, IEventStream>();
            if (_dummy) // temporary dummy mode
            {
                _blobClient = null;
                StreamContainer = null;

                DummyDataPath = ConfigurationManager.AppSettings["DummyDataPath"];
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
                _blobClient = storageAccount.CreateCloudBlobClient();
                StreamContainer = _blobClient.GetContainerReference("stream");
                StreamIndexContainer = _blobClient.GetContainerReference("streamindex");
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

        /// <summary>
        ///     The unique instance of AzureStorageClient.
        /// </summary>
        public static AzureStorageClient Instance
        {
            get
            {
                lock (_initializeLock)
                {
                    if (_singleton == null)
                        _singleton = new AzureStorageClient();    
                }
                return _singleton;
            }
        }

        /// <summary>
        ///     Get an already created event stream
        /// </summary>
        /// <param name="streamID"> The name of the stream you want.</param>
        /// <returns> The EventStream</returns>
        public IEventStream GetEventStream(String streamID) //not thread-safe yet
        {
            var stream = OpenStream(streamID);
            return stream;
        }

        /// <summary>
        ///     Create and open an EventStream
        /// </summary>
        /// <param name="streamID"> The name of the new stream. </param>
        /// <returns>The new EventStream</returns>
        public IEventStream GetNewEventStream(String streamID)
        {
            CreateEventStream(streamID);
            var stream = OpenStream(streamID);
            return stream;
        }

        /// <summary>
        ///     Open a Stream.
        /// </summary>
        /// <param name="streamID">The name of the Stream to open.</param>
        /// <returns>The EventStream.</returns>
        private IEventStream OpenStream(String streamID)
        {
            if (!StreamExists(streamID))
            {
                throw new ArgumentException("The stream " + streamID + " does not exist.");
            }

            IEventStream stream;
            if (_openedStreams.TryGetValue(streamID, out stream))
            {
                // If the stream is already opened
                return stream;
            }
            if (_dummy)
            {
                stream = new MemoryEventStream(this, streamID);
            }
            else
            {
                stream = new EventStream(this, streamID);
            }

            _openedStreams.Add(streamID, stream);
            return stream;
        }

        /// <summary>
        ///     Create an EventStream
        /// </summary>
        /// <param name="streamID">The name of the stream to create.</param>
        private void CreateEventStream(String streamID)
        {
            if (StreamExists(streamID))
                throw new ArgumentException("Stream already exists : " + streamID);
            if (_dummy)
                MemoryEventStream.CreateStream(this, streamID);
            else
                EventStream.CreateStream(this, streamID);
        }

        //missing something to close streams

        public void DeleteStreamIfExists(string streamID)
        {
            if (StreamExists(streamID))
            {
                CloseStream(streamID);
                if (_dummy)
                {
                    MemoryEventStream.DeleteStream(this, streamID);
                }
                else
                {
                    EventStream.DeleteStream(this, streamID);
                }   
            }
        }

        public void CloseStream(string streamID)
        {
            IEventStream stream;
            if (_openedStreams.TryGetValue(streamID, out stream))
            {
                _openedStreams.Remove(streamID);        // calls destructor & closes stream
                stream.Dispose();
            }
        }

        public bool StreamExists(string streamID)
        {
            if (_dummy)
            {
                return MemoryEventStream.StreamExists(this, streamID);
            }
            else
            {
                return EventStream.StreamExists(this, streamID);
            }
        }

        public bool TryGetFreshEventStream(string streamStringID, out IEventStream stream)
        {
            stream = null;
            if (StreamExists(streamStringID))
            {
                return false;
            }
            stream = GetNewEventStream(streamStringID);
            return true;
        }
    }
}
