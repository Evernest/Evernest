﻿using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
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
    class BufferedBlobIO
    {
        public byte[] WriteBuffer {get; private set;}
        public CloudPageBlob Blob {get; private set;}
        private int CurrentBufferPosition, MaximumBufferSize;
        private long CurrentPage;
        private const Int16 PageSize = 512;
        public ulong TotalWrittenBytes {get; private set;}

        public BufferedBlobIO( CloudPageBlob blob)
        {
            Blob = blob;
            byte[] buffer = new byte[sizeof(ulong)+sizeof(long)];
            Blob.DownloadRangeToByteArray(buffer, 0, 0, sizeof(ulong)+sizeof(long));
            TotalWrittenBytes = BitConverter.ToUInt64(buffer, 0);
            CurrentPage = BitConverter.ToInt64(buffer, sizeof(ulong))+1;
            int bufferSize = Int32.Parse(ConfigurationManager.AppSettings["MinimumBufferSize"]);
            MaximumBufferSize = ((bufferSize / PageSize) + 2) * PageSize;
            //WriteBuffer allocation and coherence
            WriteBuffer = new byte[MaximumBufferSize];
            CurrentBufferPosition = (int) (TotalWrittenBytes % (ulong) PageSize);
            if(CurrentBufferPosition != 0)
                Blob.DownloadRangeToByteArray(WriteBuffer, 0, (int)TotalWrittenBytes - CurrentBufferPosition, CurrentBufferPosition);
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
        static private MemoryStream MakePaddedMemoryStream(byte[] src, int offset, int count)
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
        /// Directly upload a byte array, then readies the buffer for further pushes.
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
                Blob.WritePages(stream, PageSize * CurrentPage); //should ensure no error happens
                CurrentPage += pageNumber;
                CurrentBufferPosition = count % PageSize;
                Buffer.BlockCopy(src, pageNumber * PageSize, WriteBuffer, 0, CurrentBufferPosition);
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
        private void UpdateSizeInfo()
        {
            byte[] sizeInfoBytes = new byte[PageSize];
            Buffer.BlockCopy(BitConverter.GetBytes(TotalWrittenBytes), 0, sizeInfoBytes, 0, sizeof(ulong));
            Buffer.BlockCopy(BitConverter.GetBytes(CurrentPage-1), 0, sizeInfoBytes, sizeof(ulong), sizeof(long));
            Stream sizeInfoStream = new MemoryStream(sizeInfoBytes, 0, PageSize);
            Blob.WritePages(sizeInfoStream, 0);
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
            if(CurrentBufferPosition + count > MaximumBufferSize)
            {
                int firstBatchBytes = MaximumBufferSize-CurrentBufferPosition;
                Buffer.BlockCopy(src, offset, WriteBuffer, CurrentBufferPosition, firstBatchBytes);
                CurrentBufferPosition = MaximumBufferSize;
                TotalWrittenBytes += (ulong) firstBatchBytes;
                if (FlushBuffer())
                {
                    count -= firstBatchBytes;
                    offset += firstBatchBytes;
                    if (count > MaximumBufferSize)
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
                {
                    CurrentBufferPosition -= firstBatchBytes;
                    TotalWrittenBytes -= (ulong) firstBatchBytes;
                    return false;
                }
            }
            Buffer.BlockCopy(src, offset, WriteBuffer, CurrentBufferPosition, count);
            CurrentBufferPosition += count;
            TotalWrittenBytes += (ulong) count;
            return true;
        }

        /// <summary>
        /// Retrieve a range of bytes from either the server, the buffer or both.
        /// If the byte range isn't fully available, retrieve as much byte as available.
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
            srcOffset += PageSize; //the first Page is dedicated to blob size info
            int uploadedBytes = Math.Min((int) CurrentPage*PageSize - srcOffset, count);
            if( uploadedBytes > 0 )
                Blob.DownloadRangeToByteArray(buffer, offset, srcOffset, uploadedBytes);
            Buffer.BlockCopy(WriteBuffer, Math.Max(-uploadedBytes, 0), buffer, offset+Math.Max(uploadedBytes, 0),
                Math.Min(count - Math.Max(uploadedBytes, 0), CurrentBufferPosition));
            return Math.Max(0, uploadedBytes) + Math.Min(count - uploadedBytes, CurrentBufferPosition);
        }

        /// <summary>
        /// Send pushed data over the network, empty the buffer, then readies the buffer for further pushes.
        /// </summary>
        /// <returns>
        /// true if the data was successfully sent or if the buffer was already empty
        /// false otherwise
        /// </returns>
        public bool FlushBuffer() //not thread safe, "endianness-dependant"
        {
            if (CurrentBufferPosition != 0)
            {
                if (!DirectUpload(WriteBuffer, 0, CurrentBufferPosition))
                    return false;
                UpdateSizeInfo();
            }
            return true;
        }

        //mmmh destructors are never called at the good time in this language (sigh), so not sure it's that usefull
        //IDisposable?
        ~BufferedBlobIO()
        {
            FlushBuffer();
        }
    }
}
