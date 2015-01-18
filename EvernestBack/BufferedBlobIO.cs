using System;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    internal class BufferedBlobIO
    {
        private const Int16 PageSize = 512;
        private UInt16 _currentBufferOffset = 0;
        private int _currentBufferPosition;
        private long _currentPage;
        private readonly int _maximumBufferSize;

        public BufferedBlobIO(CloudPageBlob blob, int bufferSize)
        {
            Blob = blob;
            _maximumBufferSize = ((bufferSize/PageSize) + 2)*PageSize;
            WriteBuffer = new Byte[_maximumBufferSize];
        }

        public Byte[] WriteBuffer { get; protected set; }
        public CloudPageBlob Blob { get; private set; }

        public void Push(Byte[] src, int offset, int count)
        {
            if (_currentBufferPosition + count > _maximumBufferSize)
                FlushBuffer();
            Buffer.BlockCopy(src, offset, WriteBuffer, _currentBufferPosition, count);
            _currentBufferPosition += count;
        }

        public int DownloadRangeToByteArray(Byte[] buffer, int offset, int srcOffset, int count)
        {
            var uploadedBytes = (int) _currentPage*PageSize - srcOffset;
            if (uploadedBytes > 0)
                Blob.DownloadRangeToByteArray(buffer, offset, srcOffset, uploadedBytes);
            Buffer.BlockCopy(WriteBuffer, Math.Max(-uploadedBytes, 0), buffer, offset + Math.Max(uploadedBytes, 0),
                Math.Min(count - Math.Max(uploadedBytes, 0), _currentBufferPosition));
            return Math.Max(0, uploadedBytes) + Math.Min(count - uploadedBytes, _currentBufferPosition);
        }

        public void FlushBuffer()
        {
            if (_currentBufferPosition != 0)
            {
                var pageNumber = (_currentBufferPosition/PageSize);
                Stream stream = new MemoryStream(WriteBuffer, 0, _maximumBufferSize);
                Blob.WritePages(stream, PageSize*_currentPage); //should ensure no error happens
                _currentPage += pageNumber;
                _currentBufferPosition = _currentBufferPosition%PageSize;
                Buffer.BlockCopy(WriteBuffer, pageNumber*PageSize, WriteBuffer, 0, _currentBufferPosition);
            }
        }

        ~BufferedBlobIO()
        {
            FlushBuffer();
        }
    }
}