using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    /// AzureStorageClient represents a client connection with the azure storage service 
    /// see it as an EventStream factory
    public class AzureStorageClient
    {
        private static AzureStorageClient _singleton;
        private readonly CloudBlobClient _blobClient;
        private readonly int _bufferSize;
        private readonly bool _dummy;
        private readonly uint _eventChunkSize;
        private readonly Dictionary<String, IEventStream> _openedStreams;
        private readonly long _pageBlobSize;
        private readonly CloudBlobContainer _streamContainer;
        private readonly CloudBlobContainer _streamIndexContainer;

        private AzureStorageClient()
        {
            _dummy = Boolean.Parse(ConfigurationManager.AppSettings["Dummy"]);
            _openedStreams = new Dictionary<String, IEventStream>();
            if (_dummy) // temporary dummy mode
            {
                _blobClient = null;
                _streamContainer = null;
            }
            else
            {
                // Create the blob client
                CloudStorageAccount storageAccount;
                try
                {
                    // TODO
                    var connectionString = ConfigurationManager.AppSettings["StorageAccountConnectionString"];
                    _bufferSize = Int32.Parse(ConfigurationManager.AppSettings["MinimumBufferSize"]);
                    _eventChunkSize = UInt32.Parse(ConfigurationManager.AppSettings["EventChunkSize"]);
                    _pageBlobSize = UInt32.Parse(ConfigurationManager.AppSettings["PageBlobSize"]);
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
                _streamContainer = _blobClient.GetContainerReference("stream");
                _streamIndexContainer = _blobClient.GetContainerReference("streamindex");
                try
                {
                    _streamContainer.CreateIfNotExists();
                    _streamIndexContainer.CreateIfNotExists();
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
                if (_singleton == null)
                    _singleton = new AzureStorageClient();
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
            IEventStream stream;
            if (!_openedStreams.TryGetValue(streamID, out stream))
            {
                // If OpenedStreams doesn't contains streamID, there is a problem
                throw new ArgumentException("The stream " + streamID + " does not exists.");
            }
            if (stream != null)
            {
                // If the stream is already opened
                return stream;
            }
            if (_dummy)
            {
                stream = new MemoryEventStream(streamID);
            }
            else
            {
                var streamBlob = _streamContainer.GetPageBlobReference(streamID);
                var streamIndexBlob = _streamIndexContainer.GetBlockBlobReference(streamID);
                try
                {
                    stream = new EventStream(streamBlob, streamIndexBlob, _bufferSize, _eventChunkSize);
                    // TODO : Check if EventStream throws an exception if blob references does not exists
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentException("You try to open a stream that does not exists");
                }
            }
            // Update reference, don't seem to have an "update" method... thank you microsoft
            _openedStreams.Remove(streamID);
            _openedStreams.Add(streamID, stream);
            return stream;
        }

        /// <summary>
        ///     Create an EventStream
        /// </summary>
        /// <param name="streamID">The name of the stream to create.</param>
        private void CreateEventStream(String streamID)
        {
            if (_openedStreams.ContainsKey(streamID))
            {
                // If we want to create a stream that already exists
                throw new ArgumentException("You want to create a stream with a name already used.\nStream : " +
                                            streamID);
            }
            if (_dummy)
            {
                // nothing to do
            }
            else
            {
                var streamBlob = _streamContainer.GetPageBlobReference(streamID);
                // CloudBlockBlob streamIndexBlob = _streamIndexContainer.GetBlockBlobReference(streamID);
                streamBlob.Create(_pageBlobSize);
                // TODO : Create streamIndexBlob ?
                _openedStreams.Add(streamID, null);
            }
        }

        //missing something to close streams

        public void DeleteIfExists(String streamStringID)
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