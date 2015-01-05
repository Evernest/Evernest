using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    class EventIndexer
    {
        private History Milestones;
        private UInt64 TotalWrittenBytes = 0;
        private UInt64 CurrentChunkBytes = 0;
        private UInt64 LastPosition = 0;
        private UInt32 EventChunkSizeInBytes;
        private BufferedBlobIO BufferedStreamIO;
        private CloudBlockBlob StreamIndexBlob;

        public EventIndexer( CloudBlockBlob streamIndexBlob, BufferedBlobIO buffer, UInt32 eventChunkSizeInBytes )
        {
            BufferedStreamIO = buffer;
            EventChunkSizeInBytes = eventChunkSizeInBytes;
            StreamIndexBlob = streamIndexBlob;
            Milestones = new History();
            ReadIndexInfo();
        }

        public void NotifyNewEntry(long id, UInt16 wroteBytes)
        {
            TotalWrittenBytes += wroteBytes;
            if( wroteBytes + CurrentChunkBytes > EventChunkSizeInBytes)
            {
                LastPosition += CurrentChunkBytes;
                Milestones.Insert(id, LastPosition);
                CurrentChunkBytes = wroteBytes;
            }
            else
                CurrentChunkBytes += wroteBytes;
        }

        public bool FetchEvent(long id, out String message)
        {
            return PullFromLocalCache(id, out message) || PullFromCloud(id, out message);
        }

        private bool PullFromLocalCache(long id, out String message)
        {
            message = ""; //no cache yet
            return false;
        }

        public void WriteIndexInfo()
        {
            Byte[] serializedMilestones = Milestones.Serialize();
            StreamIndexBlob.UploadFromByteArray(serializedMilestones, 0, serializedMilestones.Length);
        }

        private void ReadIndexInfo()
        {
            try
            {
                Milestones.ReadFromBlob(StreamIndexBlob);
            }
            catch(StorageException e)
            {
                Milestones.Clear();
            }
        }

        private bool PullFromCloud(long id, out String message)
        {
            UInt64 firstByte = 0;
            UInt64 lastByte = 0;
            Milestones.LowerBound(id, ref firstByte);
            if (!Milestones.UpperBound(id + 1, ref lastByte) && (lastByte = TotalWrittenBytes) == 0)
            {
                message = "";
                return false; //there's nothing written!
            }
            int byteCount = (int) (lastByte - firstByte);
            Byte[] buffer = new Byte[byteCount];
            byteCount = BufferedStreamIO.DownloadRangeToByteArray(buffer, 0, (int) firstByte, byteCount);
            UInt32 currentPosition = 0, messageLength = 0;
            long currentID = 0;
            do
            {
                currentPosition += messageLength;
                if (!BitConverter.IsLittleEndian)
                {
                    Agent.Reverse(buffer, (int)currentPosition, sizeof(long));
                    Agent.Reverse(buffer, (int)currentPosition + sizeof(UInt64), sizeof(UInt16));
                }
                currentID = BitConverter.ToInt64(buffer, (int)currentPosition);
                messageLength = BitConverter.ToUInt16(buffer, (int)currentPosition + sizeof(UInt64));
                currentPosition += sizeof(UInt64) + sizeof(UInt16);
            }
            while (currentID != id && currentPosition + messageLength < byteCount);
            message = "";
            if (currentID == id)
                message = System.Text.Encoding.Unicode.GetString(buffer, (int)currentPosition, (int)messageLength);
            return currentID == id;
        }
    }
}
