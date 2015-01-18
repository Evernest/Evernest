using System;
using System.Configuration;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    internal class EventIndexer
    {
        private ulong _currentChunkBytes;
        private DateTime _lastIndexUpdateTime;
        private ulong _lastPosition;
        private uint _newEntryCount;
        private ulong _totalWrittenBytes;
        private readonly BufferedBlobIO _bufferedStreamIo;
        private readonly uint _eventChunkSizeInBytes;
        private readonly uint _indexUpdateMinimumDelay;
        private readonly uint _indexUpdateMinimumEntryCount;
        private readonly History _milestones;
        private readonly CloudBlockBlob _streamIndexBlob;

        public EventIndexer(CloudBlockBlob streamIndexBlob, BufferedBlobIO buffer, UInt32 eventChunkSizeInBytes)
        {
            _indexUpdateMinimumEntryCount =
                UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumEntryCount"]);
            _indexUpdateMinimumDelay = UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumDelay"]);
            _bufferedStreamIo = buffer;
            _eventChunkSizeInBytes = eventChunkSizeInBytes;
            _streamIndexBlob = streamIndexBlob;
            _milestones = new History();
            ReadIndexInfo();
            _lastIndexUpdateTime = DateTime.UtcNow;
            _newEntryCount = 0;
        }

        public void NotifyNewEntry(long id, ulong wroteBytes)
        {
            _totalWrittenBytes += wroteBytes;
            if (wroteBytes + _currentChunkBytes > _eventChunkSizeInBytes)
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
                Console.WriteLine("Update!");
                UploadIndex();
            }
        }

        public void UploadIndex()
        {
            _newEntryCount = 0;
            _lastIndexUpdateTime = DateTime.UtcNow;
            var serializedMilestones = _milestones.Serialize();
            _streamIndexBlob.UploadFromByteArray(serializedMilestones, 0, serializedMilestones.Length);
        }

        private void ReadIndexInfo()
        {
            ulong position = 0;
            if (_streamIndexBlob.Exists())
            {
                _milestones.ReadFromBlob(_streamIndexBlob);
                if (_milestones.GreaterElement(ref position))
                {
                }
                _lastPosition = 0; //should retrieve last position
                //StreamIndexBlob.GetPageRange(offset, length);
            }
        }

        private bool PullFromCloud(long id, out string message)
        {
            ulong firstByte = 0;
            ulong lastByte = 0;
            _milestones.LowerBound(id, ref firstByte);
            if (!_milestones.UpperBound(id + 1, ref lastByte) && (lastByte = _totalWrittenBytes) == 0)
            {
                message = "";
                return false; //there's nothing written!
            }

            var byteCount = (int) (lastByte - firstByte);
            var buffer = new Byte[byteCount];
            byteCount = _bufferedStreamIo.DownloadRangeToByteArray(buffer, 0, (int) firstByte, byteCount);
            int currentPosition = 0, messageLength = 0;
            long currentID;
            do
            {
                currentPosition += messageLength;
                if (!BitConverter.IsLittleEndian)
                {
                    Util.Reverse(buffer, currentPosition, sizeof (long));
                    Util.Reverse(buffer, currentPosition + sizeof (UInt64), sizeof (UInt16));
                }
                currentID = BitConverter.ToInt64(buffer, currentPosition);
                messageLength = BitConverter.ToUInt16(buffer, currentPosition + sizeof (UInt64));
                currentPosition += sizeof (UInt64) + sizeof (UInt16);
            } while (currentID != id && currentPosition + messageLength < byteCount);
            message = "";
            if (currentID == id)
                message = Encoding.Unicode.GetString(buffer, currentPosition, messageLength);
            return currentID == id;
        }
    }
}