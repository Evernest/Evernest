using System;
using System.Text;
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamIndexBlob">The block blob where is the additional stream index information.</param>
        /// <param name="buffer">A lower-level interface with the page blob.</param>
        /// <param name="updateMinimumEntryCount">The minimum number of registered indexes (also referred as milestones) to be added before the indexer attempts to re-update the additional stream index information.</param>
        /// <param name="updateMinimumDelay">The minimum delay before before the indexer attempts to re-update the additional stream index information.</param>
        /// <param name="minimumChunkSize">The minimum number of bytes to be read before the indexer registers a new index (also referred as milestone).</param>
        public EventIndexer( CloudBlockBlob streamIndexBlob, BufferedBlobIO buffer, uint updateMinimumEntryCount, uint updateMinimumDelay, uint minimumChunkSize)
        {
            _indexUpdateMinimumEntryCount = updateMinimumEntryCount;
            _indexUpdateMinimumDelay = updateMinimumDelay;
            _eventChunkSizeInBytes = minimumChunkSize;
            _bufferedStreamIO = buffer;
            _streamIndexBlob = streamIndexBlob;
            _milestones = new History();
            _lastIndexUpdateTime = DateTime.UtcNow;
            _newEntryCount = 0;
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

        /// <summary>
        /// Try to retrieve an event.
        /// </summary>
        /// <param name="id">The id of the requested event.</param>
        /// <param name="message">The string to which the event's message should be written.</param>
        /// <returns>True if the event was successfully retrieved, false otherwise.</returns>
        public bool FetchEvent(long id, out string message)
        {
            return PullFromLocalCache(id, out message) || PullFromCloud(id, out message);
        }


        private bool PullFromLocalCache(long id, out string message)
        {
            message = ""; //no cache yet
            return false;
        }

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
            _streamIndexBlob.UploadFromByteArray(serializedMilestones, 0, serializedMilestones.Length);
        }

        /// <summary>
        /// Re-build the indexer from the server data.
        /// </summary>
        /// <returns>The last known event id pushed on the server.</returns>
        public long ReadIndexInfo()
        {
            long lastKnownId = 0;
            if(_streamIndexBlob.Exists())
            {
                _milestones.ReadFromBlob(_streamIndexBlob);
                if(_milestones.GreatestElement(ref _lastPosition))
                {
                    ulong lastByte = _bufferedStreamIO.TotalWrittenBytes;
                    byte[] buffer = new byte[lastByte-_lastPosition];
                    int byteCount = _bufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int) _lastPosition, (int) (lastByte - _lastPosition));
                    //should apply DRY principle, i'll make a policy-pattern-like if i have the time
                    int currentPosition = 0;
                    ulong bufferPosition = _lastPosition;
                    do
                    {
                        if (!BitConverter.IsLittleEndian)
                        {
                            Util.Reverse(buffer, currentPosition, sizeof(long));
                            Util.Reverse(buffer, currentPosition + sizeof(ulong), sizeof(int));
                        }
                        lastKnownId = BitConverter.ToInt64(buffer, currentPosition);
                        NotifyNewEntry(lastKnownId, bufferPosition+(ulong) currentPosition);
                        currentPosition += sizeof(ulong) + sizeof(int) + 
                            BitConverter.ToInt32(buffer, currentPosition + sizeof(ulong));
                    }
                    while (currentPosition + sizeof(ulong) + sizeof(int) < byteCount);
                }
            }
            return lastKnownId;
        }

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
            int currentPosition = 0, messageLength;
            long currentId;
            do
            {
                if (!BitConverter.IsLittleEndian)
                {
                    Util.Reverse(buffer, currentPosition, sizeof(long));
                    Util.Reverse(buffer, currentPosition + sizeof(ulong), sizeof(int));
                }
                currentId = BitConverter.ToInt64(buffer, currentPosition);
                messageLength = BitConverter.ToInt32(buffer, currentPosition + sizeof(ulong));
                currentPosition += sizeof(ulong) + sizeof(int) + messageLength;
            }
            while (currentId != id && currentPosition+sizeof(ulong)+sizeof(int) < byteCount);
            currentPosition -= messageLength;
            message = "";
            if (currentId == id)
                message = Encoding.Unicode.GetString(buffer, currentPosition, messageLength);
            return currentId == id;
        }

        public void Dispose()
        {
            UploadIndex();
        }
    }
}
