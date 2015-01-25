using System;
using System.Collections.Concurrent;
using System.Threading;

namespace EvernestBack
{
    /// <summary>
    /// Defines the required interface for a WorkerThread's strategy.
    /// </summary>
    /// <typeparam name="TQuery">The queries' type.</typeparam>
    /// <typeparam name="TAnswer">The answers' type.</typeparam>
    internal interface IWorkerStrategy<TQuery, TAnswer>
    {
        /// <summary>
        /// Consume a query and output an answer if possible.
        /// </summary>
        /// <param name="query">The query to be consumed.</param>
        /// <param name="answer">The answer to be retrieved.</param>
        /// <returns>True if a valid has been outputted, false otherwise.</returns>
        bool Consume(TQuery query, out TAnswer answer);
    }

    /// <summary>
    /// Consume event pushes requests and try to return the corresponding pushed events.
    /// </summary>
    internal class WriterJob : IWorkerStrategy<string, LowLevelEvent>
    {
        public long CurrentId { get; private set; }
        private readonly EventIndexer _indexer;
        private readonly BufferedBlobIO _writeBuffer;

        /// <summary>
        /// Construct the WriteJob.
        /// </summary>
        /// <param name="buffer">The (buffered) interface with the blob.</param>
        /// <param name="indexer">The indexer to notify when new events are appended to the stream.</param>
        /// <param name="firstId">The first event id to use.</param>
        public WriterJob(BufferedBlobIO buffer, EventIndexer indexer, long firstId)
        {
            _indexer = indexer;
            _writeBuffer = buffer;
            CurrentId = firstId;
        }

        public bool Consume(string query, out LowLevelEvent answer)
        {
            int size;
            answer = new LowLevelEvent(query, CurrentId);
            var bytes = answer.Serialize(out size);
            if (!_writeBuffer.Push(bytes, 0, size))
                return false;
            _indexer.NotifyNewEntry(CurrentId, (ulong)size);
            CurrentId++;
            return true;
        }
    }

    /// <summary>
    /// Consume event ranges requests and try to return the corresponding event ranges.
    /// </summary>
    internal class ReaderJob : IWorkerStrategy<Tuple<long, long>, EventRange>
    {
        private readonly EventIndexer _indexer;

        /// <summary>
        /// Construct the ReadorJob.
        /// </summary>
        /// <param name="indexer">The indexer which should be used to retrieve the events.</param>
        public ReaderJob(EventIndexer indexer)
        {
            _indexer = indexer;
        }

        public bool Consume(Tuple<long, long> query, out EventRange answer)
        {
            return _indexer.FetchEventRange(query.Item1, query.Item2, out answer);
        }
    }

    /// <summary>
    /// Host class consisting in a queue of queries consumed by a strategy class.
    /// Queries are consumed asynchronously.
    /// </summary>
    /// <typeparam name="TJob">The strategy class.</typeparam>
    /// <typeparam name="TQuery">The queries' type.</typeparam>
    /// <typeparam name="TAnswer">The answers' type.</typeparam>
    internal class WorkerThread<TJob, TQuery, TAnswer> : IDisposable
        where TJob : IWorkerStrategy<TQuery, TAnswer>
    {
        private readonly ConcurrentQueue< CallbackDecorator<TQuery, TAnswer> > _extendedQueryQueue =
            new ConcurrentQueue< CallbackDecorator<TQuery, TAnswer> >();

        private Thread _worker;
        private readonly TJob _job;
        private bool _keepOnWorking = true;
        private readonly EventWaitHandle _newTicket = new AutoResetEvent(false);

        /// <summary>
        /// Construct the host class with an instance of the corresponding strategy class.
        /// </summary>
        /// <param name="job">An instance of the strategy class.</param>
        public WorkerThread(TJob job)
        {
            _job = job;
            _worker = new Thread(WorkLoop);
            _worker.Start();
        }

        /// <summary>
        /// The main loop of the worker thread.
        /// </summary>
        private void WorkLoop()
        {
            while (_keepOnWorking)
            {
                CallbackDecorator<TQuery, TAnswer> extendedQuery;
                while (_extendedQueryQueue.TryDequeue(out extendedQuery))
                {
                    TAnswer answer;
                    if (_job.Consume(extendedQuery.Query, out answer))
                        extendedQuery.Success(answer);
                    else
                        extendedQuery.Failure("TODO");
                }
                _newTicket.WaitOne();  
            }
        }

        /// <summary>
        /// Blocks until all the pending queries are handled.
        /// </summary>
        public void Flush()
        {
            StopWorker();
            _keepOnWorking = true;
            _worker = new Thread(WorkLoop);
            _worker.Start();
        }

        /// <summary>
        /// Make the worker thread terminate and wait until does.
        /// </summary>
        private void StopWorker()
        {
            _keepOnWorking = false;
            _newTicket.Set();
            _worker.Join();
        }

        public void Dispose()
        {
            StopWorker();
        }

        /// <summary>
        /// Register a callback decorated query which will be consumed asynchronously.
        /// </summary>
        /// <param name="extendedQuery">The query to consume.</param>
        public void Register(CallbackDecorator<TQuery, TAnswer> extendedQuery)
        {
            _extendedQueryQueue.Enqueue(extendedQuery);
            _newTicket.Set();
        }
    }
}
