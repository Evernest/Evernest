using System;


namespace EvernestBack
{
    class Test
    {
        static void Main(string[] args)
        {
            UInt64 index=100;
            /*History<Int32> test = new History<Int32>();
            test.Insert(1, 1);
            test.Insert(3, 42);
            Int32 a = 0;
            test.UpperBound(2, ref a);
            Console.Write(a);*/
            AzureStorageClient asc = new AzureStorageClient(false);
            IEventStream stream = asc.GetEventStream("Test");
            stream.Push("Test", b => index = b.RequestID);
            while (true)
            {
                stream.Pull(index, b => { Console.WriteLine(b.Message + ". ID : " + b.RequestID); });
                System.Threading.Thread.Sleep(1);
            }
            //Console.Read();
            //i suspect this operation to block Console.WriteLine (thus preventing the other thread to run)
            //so i added a console.read() in the callback to have the time to see the message after pushing enter
        }
    }
}