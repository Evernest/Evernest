using System;


namespace EvernestBack
{
    class Test
    {
        static void Main(string[] args)
        {
            AzureStorageClient client = new AzureStorageClient();
            EventStream s = client.GetEventStream("test1");
            // s.Push("hello world"); TODO : BROKEN, use delegate now.
            Console.Read();
        }
    }
}