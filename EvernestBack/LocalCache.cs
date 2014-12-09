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
    class LocalCache
    {
        private History<UInt64> Milestones;
        private UInt64 TotalWrittenBytes = 0;
        private UInt64 CurrentChunkBytes = 0;
        private UInt64 LastPosition = 0;
        private UInt32 EventChunkSizeInBytes;
        private BufferedBlobIO BufferedIO;

        public LocalCache( BufferedBlobIO buffer, UInt32 eventChunkSizeInBytes )
        {
            BufferedIO = buffer;
            EventChunkSizeInBytes = eventChunkSizeInBytes;
            Milestones = new History<UInt64>();
        }

        public void NotifyNewEntry(UInt64 id, UInt16 wroteBytes)
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

        public bool FetchEvent(UInt64 id, out String message)
        {
            return PullFromLocalCache(id, out message) || PullFromCloud(id, out message);
        }

        private bool PullFromLocalCache(UInt64 id, out String message)
        {
            message = ""; //no cache yet
            return false;
        }

        private bool PullFromCloud(UInt64 id, out String message)
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
            byteCount = BufferedIO.DownloadRangeToByteArray(buffer, 0, (int) firstByte, byteCount);
            UInt32 currentPosition = 0, messageLength = 0;
            UInt64 currentID = 0;
            do
            {
                currentPosition += messageLength;
                if (!BitConverter.IsLittleEndian)
                {
                    Agent.Reverse(buffer, (int)currentPosition, (int)sizeof(UInt64));
                    Agent.Reverse(buffer, (int)currentPosition + sizeof(UInt64), (int)sizeof(UInt16));
                }
                currentID = BitConverter.ToUInt64(buffer, (int)currentPosition);
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
