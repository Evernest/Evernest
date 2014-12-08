using System;


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
            System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt");
            UInt32 counter = 0;
            while (true)
            {
                stream.Push(counter.ToString(), pushAgent =>
                {
                    stream.Pull(pushAgent.RequestID, pullAgent =>
                    {
                        Console.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                        file.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                    });
                });
                System.Threading.Thread.Sleep(100);
                counter++;
            }
            file.Close(); //this won't happen with an infinite loop, but anyway, this isn't that important

            //Console.Read();
            //i suspect this operation to block Console.WriteLine (thus preventing the other thread to run)
            //so i added a console.read() in the callback to have the time to see the message after pushing enter
        }
    }
}