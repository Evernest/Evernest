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
    class BufferedBlobIO
    {
        public Byte[] WriteBuffer {get; protected set;}
        public CloudBlockBlob Blob {get; private set;}
        private int CurrentBufferPosition, MaximumBufferSize;
        private UInt64 TotalWrittenBytes = 0;

        public BufferedBlobIO( CloudBlockBlob blob, int bufferSize )
        {
            Blob = blob;
            MaximumBufferSize = bufferSize;
            WriteBuffer = new Byte[bufferSize];
        }

        public void Push( Byte[] src, int offset, int count )
        {
            if (CurrentBufferPosition + count > MaximumBufferSize)
                FlushBuffer();
            Buffer.BlockCopy(src, 0, WriteBuffer, (int)CurrentBufferPosition, count);
            CurrentBufferPosition += count;
        }

        public int DownloadRangeToByteArray(Byte[] buffer, int offset, int srcOffset, int count)
        {
            int uploadedBytes = (int) TotalWrittenBytes - srcOffset;
            if( uploadedBytes > 0 )
                Blob.DownloadRangeToByteArray(buffer, offset, srcOffset, uploadedBytes);
            Buffer.BlockCopy(WriteBuffer, Math.Max(-uploadedBytes, 0), buffer, offset+Math.Max(uploadedBytes, 0),
                Math.Min(count - uploadedBytes, CurrentBufferPosition));
            return Math.Max(0, uploadedBytes) + Math.Min(count - uploadedBytes, CurrentBufferPosition);
        }

        public void FlushBuffer()
        {
            if (CurrentBufferPosition != 0)
            {
                Blob.UploadFromByteArray(WriteBuffer, 0, (int)CurrentBufferPosition);
                TotalWrittenBytes += (UInt64) CurrentBufferPosition;
                CurrentBufferPosition = 0;
            }
        }
    }
}
