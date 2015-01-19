using System;
using System.Threading;
using NUnit.Framework;
using EvernestBack;

namespace EvernestBackTests
{
    [TestFixture]
    public class StreamTest
    {
        public void MultiplePushAndPull(IEventStream stream, int count)
        {
            var tbl = new bool[count];
            for (var i = 0; i < count; i++)
                tbl[i] = false;
            for (var i = 0; i < count; i++)
            {
                stream.Push(i.ToString(),
                    pushAgent =>
                    {
                        stream.Pull(pushAgent.RequestID, pullAgent =>
                        {
                            Console.WriteLine(pullAgent.RequestID);
                            tbl[int.Parse(pullAgent.Message)] = true;
                        },
                            (pullAgent, message) =>
                            {
                                throw new Exception("PullAgent : " + pullAgent.RequestID + "failed with message :\n" +
                                                  message);
                            }
                            );
                    },
                    (pushAgent, message) =>
                    {
                        throw new Exception("Push Agent : " + pushAgent.RequestID + "failed with message :\n" + message);
                    });
            }
            var ok = false;
            while (!ok)
            {
                ok = true;
                for (var i = 0; i < count && ok; ok = (ok && tbl[i]), i++) ;
            }
        }

        [Test]
        public void PushAndPull()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            MultiplePushAndPull(stream, 500);
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

        [Test]
        public void TwoBatches()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            MultiplePushAndPull(stream, 100);
            Thread.Sleep(3000);
            MultiplePushAndPull(stream, 100);
        }

        [Test]
        public void Persistance()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            int count = 100;
            MultiplePushAndPull(stream, count);
            AzureStorageClient.Instance.CloseStream("TEST");
            stream = AzureStorageClient.Instance.GetEventStream("TEST");
            for (var i = 0; i < count; i++)
            {
                stream.Pull(i,
                    pullAgent =>
                    {
                        Console.WriteLine(i);
                        Assert.AreEqual(i, int.Parse(pullAgent.Message));
                    },
                    (pullAgent, message) => { Assert.Fail(); }
                    );
            }
        }
    }
}
