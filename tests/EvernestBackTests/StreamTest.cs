using System;
using System.IO;
using NUnit.Framework;
using EvernestBack;

namespace EvernestBackTests
{
    [TestFixture]
    public class StreamTest
    {
        [Test]
        public void Test1()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            const int n = 1000;
            var tbl = new bool[n];
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
            }
            var ok = false;
            while (!ok)
            {
                ok = true;
                for (var i = 0; i < 1000 && ok; ok = (ok && tbl[i]), i++) ;
            }
        }
    }
}
