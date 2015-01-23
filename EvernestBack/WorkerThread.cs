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

    internal class WriterJob : IWorkerStrategy<string, LowLevelEvent>
    {
        public long CurrentID { get; private set; }
        private readonly EventIndexer _indexer;
        private readonly BufferedBlobIO _writeBuffer;

        public WriterJob(BufferedBlobIO buffer, EventIndexer indexer, long firstId)
        {
            _indexer = indexer;
            _writeBuffer = buffer;
            CurrentID = firstId;
        }

        public bool Consume(string query, out LowLevelEvent answer)
        {
            int size;
            answer = new LowLevelEvent(query, CurrentID);
            var bytes = answer.Serialize(out size);
            if (!_writeBuffer.Push(bytes, 0, size))
                return false;
            _indexer.NotifyNewEntry(CurrentID, (ulong)size);
            CurrentID++;
            return true;
        }
    }

    internal class ReaderJob : IWorkerStrategy<Tuple<long, long>, EventRange>
    {
        private readonly EventIndexer _indexer;

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
        EventWaitHandle _newTicket = new AutoResetEvent(false);

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
            CallbackDecorator<TQuery, TAnswer> extendedQuery;
            TAnswer answer;
            while (_keepOnWorking)
            {
                while (_extendedQueryQueue.TryDequeue(out extendedQuery))
                {
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
