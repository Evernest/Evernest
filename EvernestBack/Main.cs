using System;

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
                Console.Read();
                return;
            }
            const int n = 1000;
            for (int i = 0; i < n; i++ )
            {
                stream.Push(i.ToString(), pushAgent =>
                {
                    stream.Pull(pushAgent.RequestID, pullAgent =>
                    {
                        Console.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                    });
                });
            }
            while(true)
            {}
        }
    }
}
