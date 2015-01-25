using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using EvernestBack;

namespace EvernestBackTests
{
    [TestFixture]
    public class StreamTest
    {
        static void Fail(string query, string errorMessage)
        {
            Assert.Fail();
        }

        static void Fail(long query, string errorMessage)
        {
            Assert.Fail();
        }

        static void Fail(long firstId, long lastId, string errorMessage)
        {
            Assert.Fail();
        }

        private static Random GetRNG()
        {
            int seed = /*(int) DateTime.Now.Ticks*/ 42;
            //i'm lazy so i don't want to make strings myself, change the seed if it doesn't satisfy you
            //fixed number when pushing because i want deterministic behaviour on the build server
            Console.WriteLine("Seed used : " + seed);
            return new Random(seed);
        }

        private static string RandomString(Random rng, int size)
        {
            byte[] buffer = new byte[size*2];
            rng.NextBytes(buffer);
            return Encoding.Unicode.GetString(buffer);
        }

        public void SinglePushPull(IEventStream stream, String str)
        {

            stream.Push
            (
                str,
                pushedEvent =>
                {
                    stream.Pull
                    (
                        pushedEvent.RequestId,
                        pulledEvent =>
                        {
                            Console.WriteLine(pushedEvent.RequestId);
                            Assert.AreEqual(pushedEvent.Message, pulledEvent.Message);
                            Assert.AreEqual(pushedEvent.RequestId, pulledEvent.RequestId);
                        },
                        Fail
                    );
                },
                Fail
            );
        }

        public void SinglePush(IEventStream stream, String str)
        {
            stream.Push(str, pushedEvent => {}, Fail);
        }

        public void SinglePull(IEventStream stream, long id)
        {
            stream.Pull(id,
                pulledEvent =>
                {
                    Assert.AreEqual(pulledEvent.RequestId, pulledEvent.RequestId);
                }, Fail);
        }

        public void MultiplePushAndPull(IEventStream stream, int count)
        {
            Random rng = GetRNG();
            for (int i = 0; i < count; i++)
            {
                SinglePushPull(stream, RandomString(rng, 42));
            }
        }

        [Test]
        public void PushAndPull()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            MultiplePushAndPull(stream, 500);
            AzureStorageClient.Instance.CloseStream("TEST");
        }

        [Test]
        public void BigPush()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            string str = new string(' ', UInt16.MaxValue);
            stream.Push(str,
                pushAgent =>
                {
                    stream.Pull(pushAgent.RequestId,
                        pullAgent => { Assert.AreEqual(pullAgent.Message, str); },
                        Fail );
                },
                Fail );
            AzureStorageClient.Instance.CloseStream("TEST");
        }

        [Test]
        public void TwoBatches()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            MultiplePushAndPull(stream, 100);
            Thread.Sleep(3000);
            MultiplePushAndPull(stream, 100);
            AzureStorageClient.Instance.CloseStream("TEST");
        }

        [Test]
        public void Persistance()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            int count = 100;
            Random rng = GetRNG();
            List<string> pushedStrings = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string str = RandomString(rng, 42);
                pushedStrings.Add(str);
                SinglePushPull(stream, str);
            }
            stream.FlushPushRequests();
            Assert.AreEqual((int)stream.Size(), count);
            AzureStorageClient.Instance.CloseStream("TEST");
            stream = AzureStorageClient.Instance.GetEventStream("TEST");
            Assert.AreEqual((int) stream.Size(), count);
            for (int i = 0; i < count; i++)
            {
                stream.Pull(i,
                    pulledEvent =>
                    {
                        Assert.AreEqual(pushedStrings.ElementAt((int) pulledEvent.RequestId), pulledEvent.Message);
                    },
                    Fail
                    );
            }
            AzureStorageClient.Instance.CloseStream("TEST");
        }

        [Test]
        public void PullRange()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            int count = 20;
            Random rng = GetRNG();
            List<string> pushedStrings = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string str = RandomString(rng, 42);
                pushedStrings.Add(str);
                SinglePushPull(stream, str);
            }
            stream.FlushPushRequests();
            stream.FlushPullRequests();
            stream.PullRange(5, 10,
                pulledRange =>
                {
                    //Assert.AreEqual(pulledRange.Count(), count);
                    foreach (LowLevelEvent pulledEvent in pulledRange)
                    {
                        Console.WriteLine(pulledEvent.RequestId);
                        Assert.AreEqual(pushedStrings.ElementAt((int)pulledEvent.RequestId), pulledEvent.Message);
                    }
                },
                Fail
                );
            AzureStorageClient.Instance.CloseStream("TEST");
        }

        [Test]
        public void BasicPerformanceMeasure()
        {
            AzureStorageClient.Instance.DeleteStreamIfExists("TEST");
            var stream = AzureStorageClient.Instance.GetNewEventStream("TEST");
            Random rng = GetRNG();
            const int count = 100000;
            DateTime start = DateTime.UtcNow;
            for(int i = 0 ; i < count ; i++)
                SinglePush(stream, RandomString(rng, 42));
            stream.FlushPushRequests();
            Console.WriteLine("push time ("+count+" elements):"+DateTime.UtcNow.Subtract(start).TotalMilliseconds + "ms");
            start = DateTime.UtcNow;
            for(int i = 0 ; i < count ; i++)
                SinglePull(stream, i);
            stream.FlushPullRequests();
            Console.WriteLine("pull time ("+count+" elements):"+DateTime.UtcNow.Subtract(start).TotalMilliseconds + "ms");
        }
    }
}
