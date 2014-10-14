using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Cloud14
{
    private delegate bool BookList(Producer producer);


    class WriteLocker
    {

        private Queue<Producer> writeAccessQueue = new Queue<Producer>();
        private Semaphore semaphore = new Semaphore(0, 1, "writeAccessListSemaphore");

        public WriteLocker();

        public 

        public void RequestWrite(Producer producer)
        {
            writeAccessQueue.Enqueue(producer);
        }
    }
}
