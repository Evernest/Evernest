using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Immutable;

namespace EvernestBack
{
    /// <summary>AzureStorageClient represents a client connection with the azure storage service 
    /// see it as an EventStream factory</summary>
    public class AzureStorageClient
    {
        private static readonly object InitializeLock = new object();
        private static AzureStorageClient _singleton;
        private readonly bool _dummy;
        private ImmutableDictionary<String, IEventStream> _openedStreams;
        private readonly Int32 _minimumBufferSize;
        private readonly UInt32 _eventChunkSize;
        private readonly UInt32 _updateDelay;
        private readonly Int32 _cacheSize;

        internal readonly long PageBlobSize;
        internal readonly string DummyDataPath;
        internal readonly CloudBlobContainer StreamContainer;
        internal readonly CloudBlobContainer StreamIndexContainer;

        private readonly Timer _updateTimer;

        private AzureStorageClient()
        {
            _dummy = Boolean.Parse(ConfigurationManager.AppSettings["Dummy"]);
            _openedStreams = ImmutableDictionary<String, IEventStream>.Empty;
            if (_dummy) // temporary dummy mode
            {
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
                    _updateDelay = UInt32.Parse(ConfigurationManager.AppSettings["UpdateDelay"]);
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
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                StreamContainer = blobClient.GetContainerReference("stream");
                StreamIndexContainer = blobClient.GetContainerReference("streamindex");
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
            _updateTimer = new Timer(UpdateAll, new object(),  _updateDelay, _updateDelay);
        }

        /// <summary>
        /// Close all the opened streams.
        /// </summary>
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
                lock (InitializeLock)
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
        /// <param name="streamId"> The name of the stream you want.</param>
        /// <returns> The EventStream</returns>
        public IEventStream GetEventStream(String streamId) //not thread-safe yet
        {
            var stream = OpenStream(streamId);
            return stream;
        }

        /// <summary>
        /// Get an already created EventStream
        /// </summary>
        /// <param name="streamId">The ID of the stream you want</param>
        /// <returns>The EventStream</returns>
        public IEventStream GetEventStream(Int64 streamId)
        {
            return GetEventStream("" + streamId);
        }

        /// <summary>
        ///     Create and open an EventStream
        /// </summary>
        /// <param name="streamId"> The name of the new stream. </param>
        /// <returns>The new EventStream</returns>
        public IEventStream GetNewEventStream(String streamId)
        {
            CreateEventStream(streamId);
            var stream = OpenStream(streamId);
            return stream;
        }

        /// <summary>
        ///     Create and open an EventStream
        /// </summary>
        /// <param name="streamId"> The ID of the new stream. </param>
        /// <returns>The new EventStream</returns>
        public IEventStream GetNewEventStream(Int64 streamId)
        {
            return GetNewEventStream("" + streamId);
        }

        /// <summary>
        ///     Open a Stream.
        /// </summary>
        /// <param name="streamId">The name of theEvent Stream to open.</param>
        /// <returns>The EventStream.</returns>
        private IEventStream OpenStream(String streamId)
        {
            if (!StreamExists(streamId))
                throw new ArgumentException("The stream " + streamId + " does not exist.");
            IEventStream stream;
            if (_openedStreams.TryGetValue(streamId, out stream))
            {
                // If the stream is already opened
                return stream;
            }
            if (_dummy)
            {
                stream = new MemoryEventStream(this, streamId);
            }
            else
            {
                stream = new EventStream(this, streamId, _minimumBufferSize, _eventChunkSize, _cacheSize);
            }

            _openedStreams = _openedStreams.Add(streamId, stream);
            return stream;
        }

        /// <summary>
        ///     Create an EventStream, throws if the EventStream already exists.
        /// </summary>
        /// <param name="streamId">The name of the stream to create.</param>
        private void CreateEventStream(String streamId)
        {
            if (StreamExists(streamId))
                throw new ArgumentException("Stream already exists : " + streamId);
            if (_dummy)
                MemoryEventStream.CreateStream(this, streamId);
            else
                EventStream.CreateStream(this, streamId);
        }

        /// <summary>
        /// Delete a stream if it already exists.
        /// </summary>
        /// <param name="streamId">The stream's name.</param>
        public void DeleteStreamIfExists(string streamId)
        {
            if (StreamExists(streamId))
            {
                CloseStream(streamId);
                if (_dummy)
                    MemoryEventStream.DeleteStream(this, streamId);
                else
                    EventStream.DeleteStream(this, streamId);
            }
        }

        /// <summary>
        /// Close a stream if it already exists and is already opened.
        /// </summary>
        /// <param name="streamId">The stream's name.</param>
        public void CloseStream(string streamId)
        {
            IEventStream stream;
            if (_openedStreams.TryGetValue(streamId, out stream))
            {
                _openedStreams = _openedStreams.Remove(streamId);        // calls destructor & closes stream
                stream.Dispose();
            }
        }

        /// <summary>
        /// Delete a stream if it already exists.
        /// </summary>
        /// <param name="streamId">The stream's number (will be converted to the corresponding string).</param>
        public void DeleteStreamIfExists(long streamId)
        {
            DeleteStreamIfExists(streamId.ToString());
        }

        /// <summary>
        /// Return whether the stream exists.
        /// </summary>
        /// <param name="streamId">The stream's name.</param>
        /// <returns></returns>
        public bool StreamExists(string streamId)
        {
            if (_dummy)
                return MemoryEventStream.StreamExists(this, streamId);
            return EventStream.StreamExists(this, streamId);
        }

        /// <summary>
        /// Return whether the stream exists.
        /// </summary>
        /// <param name="streamId">The stream's number (will be converted to the corresponding string).</param>
        /// <returns></returns>
        public bool StreamExists(Int64 streamId)
        {
            return StreamExists("" + streamId);
        }

        /// <summary>
        /// Try to get an existing event stream.
        /// </summary>
        /// <param name="streamStringId">The stream's name.</param>
        /// <param name="stream">The stream to be retrieved.</param>
        /// <returns>True if the stream exists, false otherwise.</returns>
        public bool TryGetExistingEventStream(string streamStringId, out IEventStream stream)
        {
            stream = null;
            if (!StreamExists(streamStringId))
                return false;
            stream = OpenStream(streamStringId);
            return true;
        }

        /// <summary>
        /// Try to get an existing event stream.
        /// </summary>
        /// <param name="streamId">The stream's Id (to be converted into a string).</param>
        /// <param name="stream">The stream to be retrieved.</param>
        /// <returns>True if the stream exists, false otherwise.</returns>
        public bool TryGetExistingEventStream(long streamId, out IEventStream stream)
        {
            return TryGetExistingEventStream(streamId.ToString(), out stream);
        }

        /// <summary>
        /// Try to create a new event stream if it doesn't already exists.
        /// </summary>
        /// <param name="streamStringId">The stream's name.</param>
        /// <param name="stream">The stream to be retrieved.</param>
        /// <returns>True if the stream didn't already exist, false otherwise.</returns>
        public bool TryGetFreshEventStream(string streamStringId, out IEventStream stream)
        {
            stream = null;
            if (StreamExists(streamStringId))
                return false;
            stream = GetNewEventStream(streamStringId);
            return true;
        }

        /// <summary>
        /// Try to create a new event stream if it doesn't already exists.
        /// </summary>
        /// <param name="streamId">The stream's number (will be converted to the corresponding string).</param>
        /// <param name="stream">The stream to be retrieved.</param>
        /// <returns>True if the stream didn't already exist, false otherwise.</returns>
        public bool TryGetFreshEventStream(long streamId, out IEventStream stream)
        {
            return TryGetFreshEventStream(streamId.ToString(), out stream);
        }

        /// <summary>
        /// Update the server for all opened streams (timer callback).
        /// </summary>
        /// <param name="o">Unused (necessary for the callback).</param>
        private void UpdateAll(object o)
        {
            ImmutableDictionary<string, IEventStream> localDictionary = _openedStreams;
            foreach (KeyValuePair<string, IEventStream> pair in localDictionary)
                pair.Value.Update();
        }

        /// <summary>
        /// Close and delete all existing streams.
        /// </summary>
        public void ClearAll()
        {
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            foreach (KeyValuePair<string, IEventStream> pair in _openedStreams)
                pair.Value.Dispose();
            _openedStreams = _openedStreams.Clear();

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
            _updateTimer.Change(_updateDelay, _updateDelay);
        }
    }
}
