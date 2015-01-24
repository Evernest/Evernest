using System;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    internal class EventIndexer:IDisposable
    {
        private History _milestones;
        private ulong _currentChunkBytes;
        private ulong _lastPosition;
        private uint _eventChunkSizeInBytes;
        private BufferedBlobIO _bufferedStreamIO;
        private CloudBlockBlob _streamIndexBlob;
        private uint _indexUpdateMinimumEntryCount;
        private uint _newEntryCount;
        private uint _indexUpdateMinimumDelay;
        private DateTime _lastIndexUpdateTime;
        private HashCache _cache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamIndexBlob">The block blob where is the additional stream index information.</param>
        /// <param name="buffer">A lower-level interface with the page blob.</param>
        /// <param name="updateMinimumEntryCount">The minimum number of registered indexes (also referred as milestones) to be added before the indexer attempts to re-update the additional stream index information.</param>
        /// <param name="updateMinimumDelay">The minimum delay before before the indexer attempts to re-update the additional stream index information.</param>
        /// <param name="minimumChunkSize">The minimum number of bytes to be read before the indexer registers a new index (also referred as milestone).</param>
        public EventIndexer( CloudBlockBlob streamIndexBlob, BufferedBlobIO buffer, uint updateMinimumEntryCount, uint updateMinimumDelay, uint minimumChunkSize, Int32 cacheSize)
        {
            _indexUpdateMinimumEntryCount = updateMinimumEntryCount;
            _indexUpdateMinimumDelay = updateMinimumDelay;
            _eventChunkSizeInBytes = minimumChunkSize;
            _bufferedStreamIO = buffer;
            _streamIndexBlob = streamIndexBlob;
            _milestones = new History();
            _lastIndexUpdateTime = DateTime.UtcNow;
            _newEntryCount = 0;
            _cache = new HashCache(cacheSize);
        }

        /// <summary>
        /// Notify the indexer a new event had been pushed.
        /// </summary>
        /// <param name="id">The id of the event.</param>
        /// <param name="wroteBytes">The size of the event in bytes.</param>
        public void NotifyNewEntry(long id, ulong wroteBytes)
        {
            if( wroteBytes + _currentChunkBytes > _eventChunkSizeInBytes)
            {
                _lastPosition += _currentChunkBytes;
                _milestones.Insert(id, _lastPosition);
                _currentChunkBytes = wroteBytes;
                _newEntryCount++;
                UploadIndexIfMeetConditions();
            }
            else
                _currentChunkBytes += wroteBytes;
        }

        /*
        /// <summary>
        /// Try to retrieve an event.
        /// </summary>
        /// <param name="id">The id of the requested event.</param>
        /// <param name="message">The string to which the event's message should be written.</param>
        /// <returns>True if the event was successfully retrieved, false otherwise.</returns>
        public bool FetchEvent(long id, out string message)
        {
            if (PullFromLocalCache(id, out message))
                return true;
            if (PullFromCloud(id, out message)) 
            {
                _cache.Add(id, message); 
                return true;
            }
            return false;
        }*/

        public bool FetchEventRange(long firstId, long lastId, out EventRange range)
        {
            return PullRangeFromCloud(firstId, lastId, out range);
        }

        private bool PullRangeFromCloud(long firstId, long lastId, out EventRange range) //DRY!!
        {
            ulong firstByte = 0;
            ulong lastByte = 0;
            _milestones.LowerBound(firstId, ref firstByte);
            if (!_milestones.UpperBound(lastId+1, ref lastByte) && (lastByte = _bufferedStreamIO.TotalWrittenBytes) == 0)
            {
                range = null;
                return false; //there's nothing written!
            }
            var byteCount = (int)(lastByte - firstByte);
            var buffer = new Byte[byteCount];
            byteCount = _bufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int)firstByte, byteCount);

            return (new EventRange(buffer, 0, byteCount)).MakeSubRange(firstId, lastId, out range);
        }

        /*
        private bool PullFromLocalCache(long id, out string message)
        {
            return _cache.Get(id, out message);
        }*/

        public void UploadIndexIfMeetConditions()
        {
            if (DateTime.UtcNow.Subtract(_lastIndexUpdateTime).TotalSeconds > _indexUpdateMinimumDelay
                && _newEntryCount > _indexUpdateMinimumEntryCount)
            {
                UploadIndex();
            }
        }

        /// <summary>
        /// Update the additional stream index information.
        /// </summary>
        public void UploadIndex()
        {
            _newEntryCount = 0;
            _lastIndexUpdateTime = DateTime.UtcNow;
            var serializedMilestones = _milestones.Serialize();
            try
            {
                _streamIndexBlob.UploadFromByteArray(serializedMilestones, 0, serializedMilestones.Length);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Re-build the indexer from the server data.
        /// </summary>
        /// <returns>The last known event id pushed on the server.</returns>
        public long ReadIndexInfo()
        {
            long nextId = 0;
            if(_streamIndexBlob.Exists())
            {
                _milestones.ReadFromBlob(_streamIndexBlob);
                if(_milestones.GreatestElement(ref _lastPosition, ref nextId))
                {
                    nextId++;
                    ulong lastByte = _bufferedStreamIO.TotalWrittenBytes;
                    if (lastByte < _lastPosition)
                    {
                        _lastPosition = 0;
                        _milestones.Clear();
                    }
                    if (lastByte > 0)
                    {
                        byte[] buffer = new byte[lastByte - _lastPosition];
                        int byteCount = _bufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int)_lastPosition, (int)(lastByte - _lastPosition));
                        EventRange unreadRange = new EventRange(buffer, 0, byteCount);
                        EventRangeEnumerator enumerator = unreadRange.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            nextId = enumerator.CurrentID;
                            NotifyNewEntry(nextId, (ulong)enumerator.CurrentSize);
                            nextId++;
                        }
                    }
                }
            }
            return nextId;
        }

        /*
        private bool PullFromCloud(long id, out string message)
        {
            ulong firstByte = 0;
            ulong lastByte = 0;
            _milestones.LowerBound(id, ref firstByte);
            if (!_milestones.UpperBound(id + 1, ref lastByte) && (lastByte = _bufferedStreamIO.TotalWrittenBytes) == 0)
            {
                message = "";
                return false; //there's nothing written!
            }
            var byteCount = (int) (lastByte - firstByte);
            var buffer = new Byte[byteCount];
            byteCount = _bufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int) firstByte, byteCount);
            EventRange unreadRange = new EventRange(buffer, 0, byteCount);
            EventRangeEnumerator enumerator = unreadRange.GetEnumerator();
            while (enumerator.MoveNext() && enumerator.CurrentID != id) ;
            if (enumerator.CurrentID == id)
            {
                message = enumerator.Current.Message;
                return true;
            }
            message = "";
            return false;
        }*/

        public void Dispose()
        {
            UploadIndex();
        }
    }
}
