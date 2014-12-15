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
        private History<long> Milestones;
        private long TotalWrittenBytes = 0;
        private long CurrentChunkBytes = 0;
        private long LastPosition = 0;
        private UInt32 EventChunkSizeInBytes;
        private BufferedBlobIO BufferedIO;

        public LocalCache( BufferedBlobIO buffer, UInt32 eventChunkSizeInBytes )
        {
            BufferedIO = buffer;
            EventChunkSizeInBytes = eventChunkSizeInBytes;
            Milestones = new History<long>();
        }

        public void NotifyNewEntry(long id, long wroteBytes)
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

        public bool FetchEvent(long id, out string message)
        {
            return PullFromLocalCache(id, out message) || PullFromCloud(id, out message);
        }

        private bool PullFromLocalCache(long id, out string message)
        {
            message = ""; //no cache yet
            return false;
        }

        private bool PullFromCloud(long id, out string message)
        {
            long firstByte = 0;
            long lastByte = 0;
            Milestones.LowerBound(id, ref firstByte);
            if (!Milestones.UpperBound(id + 1, ref lastByte) && (lastByte = TotalWrittenBytes) == 0)
            {
                message = "";
                return false; //there's nothing written!
            }
            Byte[] buffer = new Byte[lastByte - firstByte];
            BufferedIO.DownloadRangeToByteArray(buffer, 0, (int) firstByte, (int) (lastByte - firstByte));
            int currentPosition = 0, messageLength = 0;
            long currentID = 0;
            do
            {
                currentPosition += messageLength;
                if (!BitConverter.IsLittleEndian)
                {
                    Agent.Reverse(buffer, (int)currentPosition, (int)sizeof(UInt64));
                    Agent.Reverse(buffer, (int)currentPosition + sizeof(UInt64), (int)sizeof(UInt16));
                }
                currentID = BitConverter.ToInt64(buffer, currentPosition);
                messageLength = BitConverter.ToUInt16(buffer, (int)currentPosition + sizeof(UInt64));
                currentPosition += sizeof(UInt64) + sizeof(UInt16);
            }
            while (currentID != id); // /!\ should also check whether the position is still in range!
            message = System.Text.Encoding.Unicode.GetString(buffer, (int)currentPosition, (int)messageLength);
            return true;
        }
    }
}
