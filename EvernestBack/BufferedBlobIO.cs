using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

//TODO: what should we do when an exception occurs? Notifying the user? how?

namespace EvernestBack
{
    /// <summary>
    /// Low-level interface between an EventStream higher-level components and a PageBlob.
    /// Eases basic operations like retrieving or pushing data.
    /// Speeds up network transactions with buffered output.
    /// Minimizes latency between a push and the availability of corresponding data.
    /// </summary>
    internal class BufferedBlobIO:IDisposable
    {
        private readonly byte[] _writeBuffer;
        private readonly CloudPageBlob _blob;
        private int _currentBufferPosition;
        private readonly int _maximumBufferSize;
        private long _currentPage;
        private const Int16 PageSize = 512;
        public ulong TotalWrittenBytes {get; private set;}
        private readonly object _bufferLock = new object();
        private bool _bufferNeedsUpdate;

        public BufferedBlobIO( CloudPageBlob blob, int minimumBufferSize)
        {
            _blob = blob;
            byte[] buffer = new byte[sizeof(ulong)+sizeof(long)];
            _blob.DownloadRangeToByteArray(buffer, 0, 0, sizeof(ulong)+sizeof(long));
            TotalWrittenBytes = BitConverter.ToUInt64(buffer, 0);
            _currentPage = BitConverter.ToInt64(buffer, sizeof(ulong))+1;
            _maximumBufferSize = BufferSize(minimumBufferSize);
            //_writeBuffer allocation and coherence
            _writeBuffer = new byte[_maximumBufferSize];
            _currentBufferPosition = (int) (TotalWrittenBytes % (ulong) PageSize);
            if(_currentBufferPosition != 0)
                _blob.DownloadRangeToByteArray(_writeBuffer, 0, (int)TotalWrittenBytes - _currentBufferPosition + PageSize, _currentBufferPosition);
            _bufferNeedsUpdate = false;
        }

        /// <summary>
        /// Computes a well-aligned buffer size.
        /// </summary>
        /// <param name="minimumBufferSize">The minimum buffer size.</param>
        /// <returns></returns>
        private static int BufferSize(int minimumBufferSize)
        {
            return Math.Max(2*PageSize,
                minimumBufferSize%PageSize == 0 ? minimumBufferSize : ((minimumBufferSize/PageSize) + 1)*PageSize);
        }

        /// <summary>
        /// Called when an azure storage exception occurs.
        /// </summary>
        /// <param name="e">
        /// The azure storage exception.
        /// </param>
        private void HandleStorageError(StorageException e)
        {
            Console.WriteLine(e.ToString());
            //TODO : any idea?
        }

        /// <summary>
        /// Make a MemoryStream containing given data of size multiple of PageSize.
        /// </summary>
        /// <param name="src">
        /// The source byte array.
        /// </param>
        /// <param name="offset">
        /// The offset at which data should be read from the source.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be read (offset+count should be less than the source size).
        /// </param>
        /// <returns>
        /// Described MemoryStream.
        /// </returns>
        private static MemoryStream MakePaddedMemoryStream(byte[] src, int offset, int count)
        {
            MemoryStream stream;
            int padSize = (PageSize - (count % PageSize)) % PageSize;
            if (padSize != 0)
            {
                stream = new MemoryStream(count + padSize);
                stream.Write(src, offset, count);
                stream.Write(new byte[padSize], 0, padSize);
                stream.Position = 0;
            }
            else
                stream = new MemoryStream(src, offset, count);
            return stream;
        }

        /// <summary>
        /// Directly upload a byte array, then ready the buffer for further pushes.
        /// The buffer MUST have been flushed before attempting to make a direct upload
        /// (unless it is used for flushing the buffer).
        /// </summary>
        /// <param name="src">
        /// The source byte array.
        /// </param>
        /// <param name="offset">
        /// The offset at which data should be read from the source.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be read (offset+count should be less than the source size).
        /// </param>
        /// <returns>
        /// False if the upload failed, true otherwise.
        /// </returns>
        private bool DirectUpload(byte[] src, int offset, int count)
        {
            int pageNumber = count / PageSize;
            MemoryStream stream = MakePaddedMemoryStream(src, offset, count);
            try
            {
                _blob.WritePages(stream, PageSize * _currentPage); //should ensure no error happens
                _currentPage += pageNumber;
                _currentBufferPosition = count % PageSize;
                Buffer.BlockCopy(src, pageNumber * PageSize, _writeBuffer, 0, _currentBufferPosition);
            }
            catch (StorageException e)
            {
                HandleStorageError(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Update both the size of written data and the current page written at the beginning of the PageBlob.
        /// </summary>
        private void UpdateSizeInfo() //endianness-dependant
        {
            byte[] sizeInfoBytes = new byte[PageSize];
            Buffer.BlockCopy(BitConverter.GetBytes(TotalWrittenBytes), 0, sizeInfoBytes, 0, sizeof(ulong));
            Buffer.BlockCopy(BitConverter.GetBytes(_currentPage-1), 0, sizeInfoBytes, sizeof(ulong), sizeof(long));
            Stream sizeInfoStream = new MemoryStream(sizeInfoBytes, 0, PageSize);
            _blob.WritePages(sizeInfoStream, 0);
        }

        /// <summary>
        /// Push data into the buffer, may cause flushes.
        /// The data is guaranteed to be fully discarded if the push fails.
        /// </summary>
        /// <param name="src">
        /// The source byte array.
        /// </param>
        /// <param name="offset">
        /// The offset at which data should be read from the source.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be read (offset+count should be less than the source size)
        /// </param>
        /// <returns>
        /// False if the push caused a failed flush, true otherwise.
        /// </returns>
        public bool Push( byte[] src, int offset, int count )
        {
            if(_currentBufferPosition + count > _maximumBufferSize)
            {
                if (FlushBuffer())
                {
                    if (_currentBufferPosition + count > _maximumBufferSize)
                    {
                        if (DirectUpload(src, offset, count))
                        {
                            TotalWrittenBytes += (ulong) count;
                            UpdateSizeInfo();
                            return true;
                        }
                        return false;
                    }
                }
                else
                    return false;
            }
            Buffer.BlockCopy(src, offset, _writeBuffer, _currentBufferPosition, count);
            _currentBufferPosition += count;
            TotalWrittenBytes += (ulong) count;
            _bufferNeedsUpdate = true;
            return true;
        }

        /// <summary>
        /// Retrieve a range of bytes from either the server, the buffer or both.
        /// If the byte range isn't fully available, retrieve as many bytes as available.
        /// If the byte range hadn't been fully uploaded, complete it with the buffer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer in which the data should be copied.
        /// </param>
        /// <param name="offset">
        /// The offset (in bytes) at which the data should be copied in the buffer.
        /// </param>
        /// <param name="srcOffset">
        /// The offset (in bytes) at which the data should be written on the pageblob
        /// excluding special data which may be stored at the beginning of the pageblob.
        /// </param>
        /// <param name="count">
        /// The number of requested bytes (offset+count must be less than the buffer size).
        /// </param>
        /// <returns>The number of successfully read bytes</returns>
        public int DownloadRangeToByteArray(byte[] buffer, int offset, int srcOffset, int count)
        {
            lock (_bufferLock)
            {
                srcOffset += PageSize; //the first Page is dedicated to blob size info
                int uploadedBytes = Math.Min((int) _currentPage*PageSize - srcOffset, count);
                if (uploadedBytes > 0)
                    _blob.DownloadRangeToByteArray(buffer, offset, srcOffset, uploadedBytes);
                int bufferOffset = Math.Max(-uploadedBytes, 0);
                int maxReadableBytes = Math.Max(_currentBufferPosition - bufferOffset, 0);
                int bufferReadBytes = Math.Min(count - Math.Max(uploadedBytes, 0), maxReadableBytes);
                Buffer.BlockCopy(_writeBuffer, bufferOffset, buffer, offset + Math.Max(uploadedBytes, 0),
                    bufferReadBytes);
                return Math.Max(0, uploadedBytes) + bufferReadBytes;
            }
        }

        /// <summary>
        /// Send pushed data over the network, empty the buffer, then ready the buffer for further pushes.
        /// </summary>
        /// <returns>
        /// true if the data was successfully sent or if the buffer was already empty
        /// false otherwise
        /// </returns>
        public bool FlushBuffer()
        {
            if (_bufferNeedsUpdate)
            {
                lock (_bufferLock)
                {
                    if (!DirectUpload(_writeBuffer, 0, _currentBufferPosition))
                        return false;
                }
                UpdateSizeInfo();
                _bufferNeedsUpdate = false;
            }
            return true;
        }

        public void Dispose()
        {
            FlushBuffer();
        }
    }
}
