using System;
using System.Threading;
using System.IO;

namespace EvernestBack
{
    class Test
    {
        static void Main(string[] args)
        {
            /*History<Int32> test = new History<Int32>();
            test.Insert(1, 1);
            test.Insert(3, 42);
            Int32 a = 0;
            test.UpperBound(2, ref a);
            Console.Write(a);*/
            AzureStorageClient asc = new AzureStorageClient(false);
            IEventStream stream = asc.GetEventStream("Test");
            StreamWriter file = new StreamWriter("log.txt");
            UInt32 counter = 0;

            const int n = 1000;
            bool[] tbl = new bool[n];
            for (int i = 0; i < n; i++)
                tbl[i] = false;
            for (int i = 0; i < n; i++ )

            {
                stream.Push(counter.ToString(), pushAgent =>
                {
                    stream.Pull(pushAgent.RequestID, pullAgent =>
                    {
                        tbl[pullAgent.RequestID] = true;
                        Console.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                        file.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                    });
                });
                //Thread.Sleep(100);
                counter++;
            }
            bool ok = false;
            while(!ok)
            {
                ok = true;
                for( int i = 0; i < 1000 && ok; ok = ok && tbl[i], i++);
            }

            file.Close();

            //i suspect this operation to block Console.WriteLine (thus preventing the other thread to run)
            //so i added a console.read() in the callback to have the time to see the message after pushing enter
        }
    }
}
