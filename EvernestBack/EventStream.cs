using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    /// <summary>EventStream represents an instance of a stream of events and should be matched to a single blob
    /// should be created with AzureStorageClient.</summary>
    internal class EventStream : IEventStream
    {
        private CloudPageBlob _blob;
        private readonly EventIndexer _indexer;
        private readonly WriteLocker _writeLock;
        private BufferedBlobIO _bufferedIO;

        /// <summary>
        ///     Construct a new EventStream.
        /// </summary>
        public EventStream(AzureStorageClient storage, string streamID)
        {
            _blob = storage.StreamContainer.GetPageBlobReference(streamID);
            var streamIndexBlob = storage.StreamIndexContainer.GetBlockBlobReference(streamID);

            _bufferedIO = new BufferedBlobIO(_blob);
            _indexer = new EventIndexer(streamIndexBlob, _bufferedIO);
            _writeLock = new WriteLocker(_bufferedIO, _indexer, _indexer.ReadIndexInfo());
        }

        public void Dispose()
        {
            // TODO: close blob
            //_bufferedIO.Dispose();
        }

        /// <summary>
        ///     Push a message.
        /// </summary>
        /// <param name="message">The message to push. </param>
        /// <param name="callback">The action to do in case of success. </param>
        /// <param name="callbackFailure">The action to do in case of failure. </param>
        public void Push(string message, Action<IAgent> callback, Action<IAgent, String> callbackFailure)
        {
            _writeLock.Register(message, callback, callbackFailure);
        }

        /// <summary>
        ///     Pull a message.
        /// </summary>
        /// <param name="id">The index of the message to pull. </param>
        /// <param name="callback"> The action to do in case of success. </param>
        /// <param name="callbackFailure"> The action to do in case of failure. </param>
        public void Pull(long id, Action<IAgent> callback, Action<IAgent, String> callbackFailure)
        {
            string message;
            var success = _indexer.FetchEvent(id, out message);
            var msgAgent = new Agent(message, id, callback, callbackFailure);
            if (success)
            {
                msgAgent.Processed();
            }
            else
            {
                msgAgent.ProcessFailed("Can not fetch the message.");
            }
        }

        /// <summary>
        ///     Get the total number of element in the blob.
        /// </summary>
        /// <returns>Size of the blob.</returns>
        public long Size()
        {
            return _writeLock.CurrentID;
        }

        public static void CreateStream(AzureStorageClient storage, string streamID)
        {
            CloudPageBlob streamBlob = storage.StreamContainer.GetPageBlobReference(streamID);
            // CloudBlockBlob streamIndexBlob = storage.StreamIndexContainer.GetBlockBlobReference(streamID);
            streamBlob.Create(storage.PageBlobSize);
         }

        public static bool StreamExists(AzureStorageClient storage, string streamID)
        {
            return storage.StreamContainer.GetPageBlobReference(streamID).Exists();
        }

        public static void DeleteStream(AzureStorageClient storage, string streamID)
        {
            CloudPageBlob streamBlob = storage.StreamContainer.GetPageBlobReference(streamID);
            CloudBlockBlob streamIndexBlob = storage.StreamIndexContainer.GetBlockBlobReference(streamID);
            streamBlob.DeleteIfExists();
            streamIndexBlob.DeleteIfExists();
        }
    }
}
