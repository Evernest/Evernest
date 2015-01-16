using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    /// EventStream represents an instance of a stream of events and should be matched to a single blob
    /// should be created with AzureStorageClient.
    class EventStream:IEventStream
    {
        private WriteLocker WriteLock;
        private CloudBlockBlob Blob;

        private EventIndexer Indexer;

    /// <summary>
    /// Construct a new EventStream.
    /// </summary>
    /// <param name="blob"></param>
    /// <param name="streamIndexBlob"></param>
    /// <param name="bufferSize"></param>
    /// <param name="eventChunkSize"></param>
        public EventStream( CloudPageBlob blob, CloudBlockBlob streamIndexBlob, int bufferSize, uint eventChunkSize)
        {
            BufferedBlobIO buffer = new BufferedBlobIO(blob, bufferSize);
            Indexer = new EventIndexer(streamIndexBlob, buffer, eventChunkSize);
            WriteLock = new WriteLocker(buffer, Indexer, 0); // Initial ID = 0
            WriteLock.Store();
        }

    /// <summary>
    /// Push a message.
    /// </summary>
    /// <param name="message">The message to push. </param>
    /// <param name="callbackSuccess">The action to do in case of success. </param>
    /// <param name="callbackFailure">The action to do in case of failure. </param>
        public void Push(string message, Action<IAgent> callbackSuccess, Action<IAgent, String> callbackFailure)
        {
            WriteLock.Register(message, callbackSuccess, callbackFailure);
        }

    /// <summary>
    /// Pull a message.
    /// </summary>
    /// <param name="id">The index of the message to pull. </param>
    /// <param name="callbackSuccess"> The action to do in case of success. </param>
    /// <param name="callbackFailure"> The action to do in case of failure. </param>
        public void Pull(long id, Action<IAgent> callbackSuccess, Action<IAgent, String> callbackFailure)
        {
            string message;
            bool success = Indexer.FetchEvent(id, out message);
            Agent msgAgent = new Agent(message, id, callbackSuccess, callbackFailure);
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
    /// Get the total number of element in the blob.
    /// </summary>
    /// <returns>Size of the blob.</returns>
        public long Size()
        {
            return WriteLock.CurrentID;
        }

    /// <summary>
    /// Deprecated - Write the message of an agent to the console. To Remove.
    /// </summary>
    /// <param name="agent"></param>
        public void StreamDeliver(Agent agent)
        {
            Console.WriteLine(agent.Message);
        }
    }
}
