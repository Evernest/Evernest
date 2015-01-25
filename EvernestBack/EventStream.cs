using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    using Writer = WorkerThread<WriterJob, string, LowLevelEvent>;
    using Reader = WorkerThread<ReaderJob, Tuple<long, long>, EventRange>;

    /// <summary>EventStream represents an instance of a stream of events and should be matched to a single blob
    /// should be created with AzureStorageClient.</summary>
    internal class EventStream : IEventStream
    {
        private readonly EventIndexer _indexer;
        private readonly WriterJob _writerJob;
        private readonly Writer _writer;
        private readonly Reader _reader;
        private readonly BufferedBlobIO _bufferedIO;

        /// <summary>
        ///     Construct a new EventStream.
        /// </summary>
        /// <param name="storage">The stream factory.</param>
        /// <param name="streamId">The name of the stream.</param>
        /// <param name="minimumBufferSize">The minimum size of the buffer.</param>
        /// <param name="updateMinimumEntryCount">The minimum number of indexes (also referred as milestones) to be registered before re-updating the aditional stream index inforamtion.</param>
        /// <param name="updateMinimumDelay">The minimum delay before re-updating the aditional stream index inforamtion.</param>
        /// <param name="minimumChunkSize">The minimum number of bytes to be read before a new index (also referred as milestone) is registered.</param>
        /// /// <param name="cacheSize">The maximum cache size (in bytes).</param>
        public EventStream(AzureStorageClient storage, string streamId, int minimumBufferSize, uint updateMinimumEntryCount, uint updateMinimumDelay, uint minimumChunkSize, int cacheSize)
        {
            CloudPageBlob blob = storage.StreamContainer.GetPageBlobReference(streamId);
            var streamIndexBlob = storage.StreamIndexContainer.GetBlockBlobReference(streamId);

            _bufferedIO = new BufferedBlobIO(blob, minimumBufferSize);
            _indexer = new EventIndexer(streamIndexBlob, _bufferedIO, updateMinimumEntryCount, updateMinimumDelay,
                minimumChunkSize, cacheSize);
            _writerJob = new WriterJob(_bufferedIO, _indexer, _indexer.ReadIndexInfo());
            _writer = new Writer(_writerJob);
            _reader = new Reader(new ReaderJob(_indexer));
        }

        public void Dispose()
        {
            _writer.Dispose();
            _reader.Dispose();
            _indexer.Dispose();
            _bufferedIO.Dispose();
        }

        /// <summary>
        ///     Push a message.
        /// </summary>
        /// <param name="message">The message to push. </param>
        /// <param name="success">The action to do in case of success. </param>
        /// <param name="failure">The action to do in case of failure. </param>
        public void Push(string message, Action<LowLevelEvent> success, Action<string, string> failure)
        {
            _writer.Register(new CallbackDecorator<string, LowLevelEvent>(message, success, failure));
        }

        /// <summary>
        ///     Pull a message.
        /// </summary>
        /// <param name="id">The index of the message to pull. </param>
        /// <param name="success"> The action to do in case of success. </param>
        /// <param name="failure"> The action to do in case of failure. </param>
        public void Pull(long id, Action<LowLevelEvent> success, Action<long, string> failure)
        {
            _reader.Register
            (
                new CallbackDecorator<Tuple<long, long>, EventRange>
                (
                    new Tuple<long, long>(id, id),
                    range =>
                    {
                        EventRangeEnumerator enumerator = range.GetEnumerator();
                        if (enumerator.MoveNext())
                            success(enumerator.Current);
                        else
                            failure(id, "Expected singleton, got empty range.");
                    },
                    (limits, errorMessage) => { failure(limits.Item1, errorMessage); }
                )
            );
        }

        /// <summary>
        /// Pull a range of events.
        /// </summary>
        /// <param name="firstId">The first event's id.</param>
        /// <param name="lastId">The last event's id.</param>
        /// <param name="success">A callback to be called if the range is successfully pulled.</param>
        /// <param name="failure">A callback to be called if the range couldn't be pulled.</param>
        public void PullRange(long firstId, long lastId, Action<IEnumerable<LowLevelEvent>> success,
            Action<long, long, string> failure)
        {
            _reader.Register
            (
                new CallbackDecorator<Tuple<long, long>, EventRange>
                (
                    new Tuple<long, long>(firstId, lastId),
                    success,
                    (limits, errorMessage) => { failure(limits.Item1, limits.Item2, errorMessage); }
                )
            );
        }

        /// <summary>
        /// Blocks until all push requests are handled (includes events pushed during the execution of FlushPushes).
        /// </summary>
        public void FlushPushRequests()
        {
            _writer.Flush();
        }

        /// <summary>
        /// Blocks until all pull requests are handled (includes events pushed during the execution of FlushPushes).
        /// </summary>
        public void FlushPullRequests()
        {
            _reader.Flush();
        }

        /// <summary>
        ///     Get the total number of elements in the stream.
        /// </summary>
        /// <returns>Size of the blob.</returns>
        public long Size()
        {
            return _writerJob.CurrentId;
        }

        /// <summary>
        /// Create a stream.
        /// </summary>
        /// <param name="storage">The stream manager.</param>
        /// <param name="streamId">The stream's name.</param>
        public static void CreateStream(AzureStorageClient storage, string streamId)
        {
            CloudPageBlob streamBlob = storage.StreamContainer.GetPageBlobReference(streamId);
            streamBlob.Create(storage.PageBlobSize);
         }

        /// <summary>
        /// Tell whether the stream exists or not.
        /// </summary>
        /// <param name="storage">The stream manager.</param>
        /// <param name="streamId">The stream's name.</param>
        /// <returns></returns>
        public static bool StreamExists(AzureStorageClient storage, string streamId)
        {
            return storage.StreamContainer.GetPageBlobReference(streamId).Exists();
        }

        /// <summary>
        /// Delete a stream if it already exists.
        /// </summary>
        /// <param name="storage">The stream manager.</param>
        /// <param name="streamId">The stream's name.</param>
        public static void DeleteStream(AzureStorageClient storage, string streamId)
        {
            CloudPageBlob streamBlob = storage.StreamContainer.GetPageBlobReference(streamId);
            CloudBlockBlob streamIndexBlob = storage.StreamIndexContainer.GetBlockBlobReference(streamId);
            streamIndexBlob.DeleteIfExists();
            streamBlob.DeleteIfExists();
        }
    }
}
