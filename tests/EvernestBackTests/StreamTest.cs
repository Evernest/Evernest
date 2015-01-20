using System;
using System.Threading;
using NUnit.Framework;
using EvernestBack;

namespace EvernestBackTests
{
    [TestFixture]
    public class StreamTest
    {
        static void Fail(LowLevelEvent e, string errorMessage)
        {
            Assert.Fail();
        }

        static void Fail(LowLevelEvent e)
        {
            Assert.Fail();
        }

        static void AssertCorrect(LowLevelEvent e, string expected)
        {
            Assert.AreEqual(e.Message, expected);
        }

        public void MultiplePushAndPull(IEventStream stream, int count)
        {
            for (var i = 0; i < count; i++)
            {
                stream.Push(i.ToString(),
                    pushAgent =>
                    {
                        stream.Pull(pushAgent.RequestID, 
                            pullAgent=>{Assert.AreEqual(pushAgent.Message, pullAgent.Message);},
                            Fail);
                    }, Fail );
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
            bool success = false, done = false;
            stream.Push(str,
                pushAgent =>
                {
                    stream.Pull(pushAgent.RequestID,
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
                    Fail
                    );
            }
            AzureStorageClient.Instance.CloseStream("TEST");
        }
    }
}
