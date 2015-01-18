using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    /// EventStream represents an instance of a stream of events and should be matched to a single blob
    /// should be created with AzureStorageClient.
    internal class EventStream : IEventStream
    {
        private CloudBlockBlob _blob;
        private readonly EventIndexer _indexer;
        private readonly WriteLocker _writeLock;

        /// <summary>
        ///     Construct a new EventStream.
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="streamIndexBlob"></param>
        /// <param name="bufferSize"></param>
        /// <param name="eventChunkSize"></param>
        public EventStream(CloudPageBlob blob, CloudBlockBlob streamIndexBlob, int bufferSize, uint eventChunkSize)
        {
            var buffer = new BufferedBlobIO(blob, bufferSize);
            _indexer = new EventIndexer(streamIndexBlob, buffer, eventChunkSize);
            _writeLock = new WriteLocker(buffer, _indexer, 0); // Initial ID = 0
            _writeLock.Store();
        }

        /// <summary>
        ///     Push a message.
        /// </summary>
        /// <param name="message">The message to push. </param>
        /// <param name="callbackSuccess">The action to do in case of success. </param>
        /// <param name="callbackFailure">The action to do in case of failure. </param>
        public void Push(string message, Action<IAgent> callbackSuccess, Action<IAgent, String> callbackFailure)
        {
            _writeLock.Register(message, callbackSuccess, callbackFailure);
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
    }
}