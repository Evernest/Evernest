using System;
using System.Configuration;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    internal class EventIndexer
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

        public EventIndexer( CloudBlockBlob streamIndexBlob, BufferedBlobIO buffer)
        {
            _indexUpdateMinimumEntryCount = UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumEntryCount"]);
            _indexUpdateMinimumDelay = UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumDelay"]);
            _bufferedStreamIO = buffer;
            _eventChunkSizeInBytes = UInt32.Parse(ConfigurationManager.AppSettings["EventChunkSize"]); ;
            _streamIndexBlob = streamIndexBlob;
            _milestones = new History();
            ReadIndexInfo();
            _lastIndexUpdateTime = DateTime.UtcNow;
            _newEntryCount = 0;
        }

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

        public void UploadIndex()
        {
            _newEntryCount = 0;
            _lastIndexUpdateTime = DateTime.UtcNow;
            var serializedMilestones = _milestones.Serialize();
            _streamIndexBlob.UploadFromByteArray(serializedMilestones, 0, serializedMilestones.Length);
        }

        private void ReadIndexInfo()
        {
            if(_streamIndexBlob.Exists())
            {
                _milestones.ReadFromBlob(_streamIndexBlob);
                if(_milestones.GreaterElement(ref _lastPosition))
                {
                    //should retrieve currentChunkBytes
                }
            }
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
            int currentPosition = 0, messageLength = 0;
            long currentId;
            do
            {
                if (!BitConverter.IsLittleEndian)
                {
                    Util.Reverse(buffer, currentPosition, sizeof(long));
                    Util.Reverse(buffer, currentPosition + sizeof(ulong), sizeof(ushort));
                }
                currentId = BitConverter.ToInt64(buffer, currentPosition);
                messageLength = BitConverter.ToUInt16(buffer, currentPosition + sizeof(ulong));
                currentPosition += sizeof(ulong) + sizeof(ushort) + messageLength;
            }
            while (currentId != id && currentPosition+sizeof(ulong)+sizeof(ushort) < byteCount);
            currentPosition -= messageLength;
            message = "";
            if (currentId == id)
                message = System.Text.Encoding.Unicode.GetString(buffer, currentPosition, messageLength);
            return currentId == id;
        }
    }
}
