using System;


namespace EvernestBack
{
    class Test
    {
        static void Main(string[] args)
        {
            /*UInt64 index=100;
            RAMStream Stream = new RAMStream();
            Stream.Push("Test", a => index = a.RequestID);
            Stream.Pull(index, a => Console.WriteLine(a.Message + ". ID : " + a.RequestID));
            Console.Read();*/
            History<Int32> test = new History<Int32>();
            test.Insert(1, 1);
            test.Insert(3, 42);
            Int32 a = 0;
            test.UpperBound(2, ref a);
            Console.Write(a);
            AzureStorageClient asc = new AzureStorageClient();
            IStream s = asc.GetEventStream("Test");
        }
    }
}