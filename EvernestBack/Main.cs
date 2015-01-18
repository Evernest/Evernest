using System;
using System.IO;

namespace EvernestBack
{
    internal class Test
    {
        private static void Main(string[] args)
        {
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");

            var file = new StreamWriter("log.txt");

            const int n = 1000;
            var tbl = new bool[n + 500];
            for (var i = 0; i < n; i++)
                tbl[i] = false;
            for (var i = 0; i < n; i++)
            {
                stream.Push(i.ToString(),
                    pushAgent =>
                    {
                        stream.Pull(pushAgent.RequestID, pullAgent =>
                        {
                            tbl[pullAgent.RequestID] = true;
                            Console.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                            file.WriteLine(pullAgent.Message + ". ID : " + pullAgent.RequestID);
                        },
                            (pullAgent, message) =>
                            {
                                Console.WriteLine("PullAgent : " + pullAgent.RequestID + "failed with message :\n" +
                                                  message);
                            }
                            );
                    },
                    (pushAgent, message) =>
                    {
                        Console.WriteLine("Push Agent : " + pushAgent.RequestID + "failed with message :\n" + message);
                    });
                //System.Threading.Thread.Sleep(100);
            } //this won't happen with an infinite loop, but anyway, this isn't that important
            var ok = false;
            while (!ok)
            {
                ok = true;
                for (var i = 0; i < 1000 && ok; ok = (ok && tbl[i]), i++)
                {
                }
            }

            file.Close();

            //I suspect this operation to block Console.WriteLine (thus preventing the other thread to run)
            //so I added a console.read() in the callback to have the time to see the message after pushing enter
        }
    }
}