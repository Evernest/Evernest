using System;
using NUnit.Framework;
using EvernestBack;

namespace EvernestBackTests
{
    [TestFixture]
    public class StreamTest
    {
        [Test]
        public void PushAndPull()
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
                            Console.WriteLine(pullAgent.RequestID);
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

        [Test]
        public void BigPush()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            string str = new string(' ', UInt16.MaxValue);
            bool success = false, done = false;
            stream.Push(str,
                pushAgent =>
                {
                    stream.Pull(pushAgent.RequestID,
                        pullAgent =>
                        {
                            success = pullAgent.Message == str;
                            done = true;
                        },
                        (pullAgent, message) => { done = true; }
                        );
                },
                (pushAgent, message) => { done = true; }
                );
            while (!done) ;
            Assert.IsTrue(success);
        }

    }
}
