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
                stream = AzureStorageClient.singleton.GetNewEventStream("Test");
            }
            catch (ArgumentException e)
            {
                Console.Write(e.Message);
                return;
            }
            System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt");
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
                //System.Threading.Thread.Sleep(100);
                counter++;
            } //this won't happen with an infinite loop, but anyway, this isn't that important
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
