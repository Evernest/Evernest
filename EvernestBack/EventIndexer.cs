﻿using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    internal class EventIndexer:IDisposable
    {
        private History _milestones;
        private bool _milestonesNeedUpdate;
        private ulong _currentChunkBytes;
        private ulong _lastPosition;
        private readonly uint _eventChunkSizeInBytes;
        private readonly BufferedBlobIO _bufferedStreamIO;
        private readonly CloudBlockBlob _streamIndexBlob;
        private readonly RangeCache _cache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamIndexBlob">The block blob where is the additional stream index information.</param>
        /// <param name="buffer">A lower-level interface with the page blob.</param>
        /// <param name="minimumChunkSize">The minimum number of bytes to be read before the indexer registers a new index (also referred as milestone).</param>
        /// <param name="cacheSize">The cache size in bytes.</param>
        public EventIndexer( CloudBlockBlob streamIndexBlob, BufferedBlobIO buffer, uint minimumChunkSize, int cacheSize)
        {
            _milestonesNeedUpdate = false;
            _eventChunkSizeInBytes = minimumChunkSize;
            _bufferedStreamIO = buffer;
            _streamIndexBlob = streamIndexBlob;
            _milestones = new History();
            _cache = new RangeCache(cacheSize);
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
                _milestonesNeedUpdate = true;
            }
            else
                _currentChunkBytes += wroteBytes;
        }

        /// <summary>
        /// Try to retrieve an event range.
        /// </summary>
        /// <param name="firstId">The first event's id.</param>
        /// <param name="lastId">The last event's id.</param>
        /// <param name="range">The range to be retrieved.</param>
        /// <returns>True if the range was successfully retrieved, false otherwise.</returns>
        public bool FetchEventRange(long firstId, long lastId, out EventRange range)
        {
            return _cache.TrySetRange(firstId, lastId, out range) || PullRangeFromCloud(firstId, lastId, out range);
        }

        /// <summary>
        /// Try to retrieve an event range from the server.
        /// </summary>
        /// <param name="firstId">The first event's id.</param>
        /// <param name="lastId">The last event's id.</param>
        /// <param name="range">The range to be retrieved.</param>
        /// <returns>True if the range was successfully retrieved, false otherwise.</returns>
        private bool PullRangeFromCloud(long firstId, long lastId, out EventRange range) //DRY!!
        {
            ulong firstByte = 0;
            ulong lastByte = 0;
            long lastRetrievedKey = 0;
            _milestones.LowerBound(firstId, ref firstByte);
            if (!_milestones.UpperBound(lastId+1, ref lastByte, ref lastRetrievedKey) && (lastByte = _bufferedStreamIO.TotalWrittenBytes) == 0)
            {
                range = null;
                return false; //there's nothing written!
            }
            if (lastRetrievedKey == 0)
                lastRetrievedKey = lastId + 1;
            else
                lastRetrievedKey--;
            var byteCount = (int)(lastByte - firstByte);
            var buffer = new Byte[byteCount];
            var realByteCount = _bufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int)firstByte, byteCount);
            EventRange superRange = new EventRange(buffer, 0, realByteCount);
            if (byteCount == realByteCount) //shouldn't be necessary, but who knows?
                _cache.InsertRange(superRange, firstId, lastRetrievedKey);
            return superRange.MakeSubRange(firstId, lastId, out range);
        }

        /// <summary>
        /// Update the additional stream index information.
        /// </summary>
        public void UploadIndex()
        {
            if(_milestonesNeedUpdate)
            {
                var serializedMilestones = _milestones.Serialize();
                try
                {
                    _streamIndexBlob.UploadFromByteArray(serializedMilestones, 0, serializedMilestones.Length);
                    _milestonesNeedUpdate = false;
                }
                catch (StorageException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Re-build the indexer from the server data.
        /// </summary>
        /// <returns>The last known event id pushed on the server.</returns>
        public long ReadIndexInfo()
        {
            long nextId = 0;
            ulong lastByte = _bufferedStreamIO.TotalWrittenBytes;
            if(_streamIndexBlob.Exists())
            {
                _milestones = new History(_streamIndexBlob);
                if(_milestones.GreatestElement(ref _lastPosition, ref nextId))
                {
                    nextId++;
                    if (lastByte < _lastPosition)
                    {
                        _lastPosition = 0;
                        _milestones = new History();
                    }
                }
            }
            if (lastByte > 0)
            {
                byte[] buffer = new byte[lastByte - _lastPosition];
                int byteCount = _bufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int)_lastPosition, (int)(lastByte - _lastPosition));
                EventRange unreadRange = new EventRange(buffer, 0, byteCount);
                EventRangeEnumerator enumerator = unreadRange.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    nextId = enumerator.CurrentId;
                    NotifyNewEntry(nextId, (ulong)enumerator.CurrentSize);
                    nextId++;
                }
            }
            return nextId;
        }

        public void Dispose()
        {
            UploadIndex();
        }
    }
}
