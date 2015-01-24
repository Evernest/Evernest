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
        private readonly Int32 _minimumBufferSize;
        private readonly UInt32 _eventChunkSize;
        private readonly UInt32 _indexUpdateMinimumEntryCount;
        private readonly UInt32 _indexUpdateMinimumDelay;
        private readonly Int32 _cacheSize;

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
                    var connectionString = ConfigurationManager.AppSettings["StorageAccountConnectionString"];
                    PageBlobSize = UInt32.Parse(ConfigurationManager.AppSettings["PageBlobSize"]);
                    _minimumBufferSize = Int32.Parse(ConfigurationManager.AppSettings["MinimumBufferSize"]);
                    _eventChunkSize = UInt32.Parse(ConfigurationManager.AppSettings["EventChunkSize"]);
                    _indexUpdateMinimumEntryCount = UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumentryCount"]);
                    _indexUpdateMinimumDelay = UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumDelay"]);
                    _cacheSize = Int32.Parse(ConfigurationManager.AppSettings["CacheSize"]);
                    storageAccount = CloudStorageAccount.Parse(connectionString);
                }
                catch (NullReferenceException e)
                {
                    Console.Error.WriteLine("StorageAccount configuration error:");
                    Console.Error.WriteLine("Method : {0}", e.TargetSite);
                    Console.Error.WriteLine("Message : {0}", e.Message);
                    Console.Error.WriteLine("Source : {0}", e.Source);
                    throw;
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

        private void CloseAll()
        {
            // Properly dispose of each event stream so that the data is saved.
            foreach (KeyValuePair<string, IEventStream> pair in _openedStreams)
                pair.Value.Dispose();
        }

        public static void Close()
        {
            Instance.CloseAll();
            _singleton = null;
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
        ///     Get an already created EventStream
        /// </summary>
        /// <param name="streamID"> The name of the stream you want.</param>
        /// <returns> The EventStream</returns>
        public IEventStream GetEventStream(String streamID) //not thread-safe yet
        {
            var stream = OpenStream(streamID);
            return stream;
        }

        /// <summary>
        /// Get an already created EventStream
        /// </summary>
        /// <param name="streamID">The ID of the stream you want</param>
        /// <returns>The EventStream</returns>
        public IEventStream GetEventStream(Int64 streamID)
        {
            return GetEventStream("" + streamID);
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
        ///     Create and open an EventStream
        /// </summary>
        /// <param name="streamID"> The ID of the new stream. </param>
        /// <returns>The new EventStream</returns>
        public IEventStream GetNewEventStream(Int64 streamID)
        {
            return GetNewEventStream("" + streamID);
        }

        /// <summary>
        ///     Open a Stream.
        /// </summary>
        /// <param name="streamID">The name of theEvent Stream to open.</param>
        /// <returns>The EventStream.</returns>
        private IEventStream OpenStream(String streamID)
        {
            if (!StreamExists(streamID))
                throw new ArgumentException("The stream " + streamID + " does not exist.");
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
                stream = new EventStream(this, streamID, _minimumBufferSize, 
                    _indexUpdateMinimumEntryCount, _indexUpdateMinimumDelay, _eventChunkSize, _cacheSize);
            }

            _openedStreams.Add(streamID, stream);
            return stream;
        }

        /// <summary>
        ///     Create an EventStream, throws if the EventStream already exists.
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

        public void DeleteStreamIfExists(string streamID)
        {
            if (StreamExists(streamID))
            {
                CloseStream(streamID);
                if (_dummy)
                    MemoryEventStream.DeleteStream(this, streamID);
                else
                    EventStream.DeleteStream(this, streamID);
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

        public void DeleteStreamIfExists(Int64 streamID)
        {
            DeleteStreamIfExists("" + streamID);
        }

        public bool StreamExists(string streamID)
        {
            if (_dummy)
                return MemoryEventStream.StreamExists(this, streamID);
            return EventStream.StreamExists(this, streamID);
        }

        public bool StreamExists(Int64 streamID)
        {
            return StreamExists("" + streamID);
        }

        public bool TryGetFreshEventStream(string streamStringID, out IEventStream stream)
        {
            stream = null;
            if (StreamExists(streamStringID))
                return false;
            stream = GetNewEventStream(streamStringID);
            return true;
        }


        public bool TryGetFreshEventStream(Int64 streamID, out IEventStream stream)
        {
            return TryGetFreshEventStream("" + streamID, out stream);
        }

        public void ClearAll()
        {
            foreach (KeyValuePair<string, IEventStream> pair in _openedStreams)
                pair.Value.Dispose();
            _openedStreams.Clear();

            if (_dummy)
            {
                MemoryEventStream.ClearAll(this);
            }
            else
            {
                StreamIndexContainer.DeleteIfExists();
                StreamContainer.DeleteIfExists();
                StreamIndexContainer.CreateIfNotExists();
                StreamContainer.CreateIfNotExists();
            }
        }
    }
}
