using System;
using System.Threading;
using System.IO;

namespace EvernestBack
{
    class Test
    {
        static void Main(string[] args)
        {
            IEventStream stream;
            try
            {
                stream = AzureStorageClient.Instance.GetNewEventStream("Test");
            }
            catch (ArgumentException e)
            {
                Console.Write(e.Message);
                return;
            }
            System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt");

            const int n = 1000;
            for (int i = 0; i < n; i++ )
            {
                stream.Push(i.ToString(), pushAgent =>
                {
                    stream.Pull(pushAgent.RequestID, pullAgent =>
                    {
                        Console.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                        file.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                    });
                });
                System.Threading.Thread.Sleep(100);
            }
            while(true)
            {}

            //file.Close();
        }
    }
}
