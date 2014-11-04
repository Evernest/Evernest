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
        private CloudBlobClient blobClient;

        public WriteLocker(CloudBlobClient blobClient)
        {
            this.blobClient = blobClient;
            Store();
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
            // TODO
            Console.WriteLine("Store producer {0} to Cloud.", prod.requestID);
            Console.WriteLine(prod.message);
        }

        public void Register(Producer producer)
        {
            Task.Run(() => { waitingProducers.Add(producer); });
        }
        

	}
}
