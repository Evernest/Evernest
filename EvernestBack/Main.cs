using System;


namespace EvernestBack
{
    class Test
    {
        static void Main(string[] args)
        {
            //UInt64 index=100;
            //RAMStream Stream = new RAMStream();
            //Stream.Push("Test", a => index = a.RequestID);
            //Stream.Pull(index, a => Console.WriteLine(a.Message + ". ID : " + a.RequestID));
            //Console.Read();
            AzureStorageClient asc = new AzureStorageClient();
            IStream s = asc.GetEventStream("Test");
            s.Push("Test", a => Console.WriteLine("Message poussé. Id : " + a.RequestID));
        }
    }
}