using System;
using Microsoft.WindowsAzure.Storage.Blob;



namespace EvernestBack
{
    class BufferedBlobIO
    {
        public Byte[] WriteBuffer {get; protected set;}
        public CloudPageBlob Blob {get; private set;}
        private int CurrentBufferPosition, MaximumBufferSize;
        private long CurrentPage = 0;
        private UInt16 CurrentBufferOffset = 0;
        private const Int16 PageSize = 512;

        public BufferedBlobIO( CloudPageBlob blob, int bufferSize )
        {
            Blob = blob;
            MaximumBufferSize = ((bufferSize/PageSize)+2)*PageSize;
            WriteBuffer = new Byte[MaximumBufferSize];
        }

        public void Push( Byte[] src, int offset, int count )
        {
            if (CurrentBufferPosition + count > MaximumBufferSize)
                FlushBuffer();
            Buffer.BlockCopy(src, offset, WriteBuffer, (int)CurrentBufferPosition, count);
            CurrentBufferPosition += count;
        }

        public int DownloadRangeToByteArray(Byte[] buffer, int offset, int srcOffset, int count)
        {
            int uploadedBytes = (int) CurrentPage*PageSize - srcOffset;
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
                int pageNumber = (CurrentBufferPosition/PageSize);
                System.IO.Stream stream = new System.IO.MemoryStream(WriteBuffer, 0, MaximumBufferSize);
                Blob.WritePages(stream, PageSize*CurrentPage, null, null, null, null); //should ensure no error happens
                CurrentPage += pageNumber;
                CurrentBufferPosition = CurrentBufferPosition % PageSize;
                Buffer.BlockCopy(WriteBuffer, (int) pageNumber*PageSize, WriteBuffer, 0, CurrentBufferPosition);
            }
        }
    }
}
