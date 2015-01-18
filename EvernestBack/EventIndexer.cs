using System;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace EvernestBack
{
    class EventIndexer
    {
        private History Milestones;
        private ulong CurrentChunkBytes = 0;
        private ulong LastPosition = 0;
        private uint EventChunkSizeInBytes;
        private BufferedBlobIO BufferedStreamIO;
        private CloudBlockBlob StreamIndexBlob;
        private uint IndexUpdateMinimumEntryCount;
        private uint NewEntryCount;
        private uint IndexUpdateMinimumDelay;
        private DateTime LastIndexUpdateTime;

        public EventIndexer( CloudBlockBlob streamIndexBlob, BufferedBlobIO buffer)
        {
            IndexUpdateMinimumEntryCount = UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumEntryCount"]);
            IndexUpdateMinimumDelay = UInt32.Parse(ConfigurationManager.AppSettings["IndexUpdateMinimumDelay"]);
            BufferedStreamIO = buffer;
            EventChunkSizeInBytes = UInt32.Parse(ConfigurationManager.AppSettings["EventChunkSize"]); ;
            StreamIndexBlob = streamIndexBlob;
            Milestones = new History();
            ReadIndexInfo();
            LastIndexUpdateTime = DateTime.UtcNow;
            NewEntryCount = 0;
        }

        public void NotifyNewEntry(long id, ulong wroteBytes)
        {
            if( wroteBytes + CurrentChunkBytes > EventChunkSizeInBytes)
            {
                LastPosition += CurrentChunkBytes;
                Milestones.Insert(id, LastPosition);
                CurrentChunkBytes = wroteBytes;
                NewEntryCount++;
                UploadIndexIfMeetConditions();
            }
            else
                CurrentChunkBytes += wroteBytes;
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
            if (DateTime.UtcNow.Subtract(LastIndexUpdateTime).TotalSeconds > IndexUpdateMinimumDelay
                && NewEntryCount > IndexUpdateMinimumEntryCount )
            {
                UploadIndex();
            }

        }

        public void UploadIndex()
        {
            NewEntryCount = 0;
            LastIndexUpdateTime = DateTime.UtcNow;
            Byte[] serializedMilestones = Milestones.Serialize();
            StreamIndexBlob.UploadFromByteArray(serializedMilestones, 0, serializedMilestones.Length);
        }

        private void ReadIndexInfo()
        {
            if(StreamIndexBlob.Exists())
            {
                Milestones.ReadFromBlob(StreamIndexBlob);
                if(Milestones.GreaterElement(ref LastPosition))
                {
                    //should retrieve currentChunkBytes
                }
            }
        }

        private bool PullFromCloud(long id, out string message)
        {
            ulong firstByte = 0;
            ulong lastByte = 0;
            Milestones.LowerBound(id, ref firstByte);
            if (!Milestones.UpperBound(id + 1, ref lastByte) && (lastByte = BufferedStreamIO.TotalWrittenBytes) == 0)
            {
                message = "";
                return false; //there's nothing written!
            }

            int byteCount = (int) (lastByte - firstByte);
            Byte[] buffer = new Byte[byteCount];
            byteCount = BufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int) firstByte, byteCount);
            int currentPosition = 0, messageLength = 0;
            long currentId;
            do
            {
                if (!BitConverter.IsLittleEndian)
                {
                    Agent.Reverse(buffer, currentPosition, sizeof(long));
                    Agent.Reverse(buffer, currentPosition + sizeof(ulong), sizeof(ushort));
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
