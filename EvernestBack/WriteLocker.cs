using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;


namespace EvernestBack
{
    class WriteLocker
	{

		private BlockingCollection<Producer> waitingProducers = new BlockingCollection<Producer>();
        private CloudBlockBlob blob;
        private CloudBlobStream outputStream;

        public WriteLocker(CloudBlockBlob blob)
        {
            this.blob = blob;
            blob.StreamWriteSizeInBytes = 65536; //64KiB for now, totally arbitrary
            outputStream = blob.OpenWrite();
        }

        public void Store()
        {
            Task.Run(() =>
            {
                Console.WriteLine("Starting Storing");
                while (waitingProducers.Count > 0)
                {
                    Producer p = waitingProducers.Take();
                    StoreToCloud(p);
                    p.Processed();
                }
            }
            );
        }

        private void StoreToCloud(Producer prod)
        {
            UInt16 size;
            Byte[] bytes = prod.Serialize(out size);
            outputStream.Write(bytes, 0, size);
        }

        public void Register(Producer producer)
        {
            Task.Run(() => { waitingProducers.Add(producer); });
        }
        

	}
}
