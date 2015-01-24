using System;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;
using NUnit.Framework;

namespace EvernestFrontTests.UtilitiesTests
{
    [TestFixture]
    class SerializerTests
    {
        private const string Message = "message";
        

        [Test]
        public static void ReadContract_Success()
        {
            var serializer = new Serializer();
            var userId = UserTests.AddUser_GetId_AssertSuccess(Helpers.NewName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var date = DateTime.UtcNow;
            var contract = new EventContract(user.Name, user.Id, date, Message);
            var serializedContract = serializer.WriteContract<EventContract>(contract);
            var deserializedContract = serializer.ReadContract<EventContract>(serializedContract);
            Assert.AreEqual(contract.AuthorId, deserializedContract.AuthorId);
            Assert.AreEqual(contract.AuthorName, deserializedContract.AuthorName);
            Assert.AreEqual(contract.Date, deserializedContract.Date);
            Assert.AreEqual(contract.Message, deserializedContract.Message);
        }

        [Test]
        public static void ReadSystemEventEnvelope_Success()
        {
            var serializer = new Serializer();
            const string userName = "user";
            const string streamName = "stream";
            const int streamId = 42;
            const int userId = 32;
            var systemEvent = new EventStreamCreatedSystemEvent(streamId, streamName, userName, userId);
            var envelope = new SystemEventEnvelope(systemEvent.GetType().Name, serializer.WriteContract(systemEvent));
            var serializedContract = serializer.WriteContract(envelope);
            var deserializedContract = serializer.ReadSystemEventEnvelope(serializedContract);
            Assert.IsAssignableFrom<EventStreamCreatedSystemEvent>(deserializedContract);
            var deserializedEvent = (EventStreamCreatedSystemEvent) deserializedContract;
            Assert.AreEqual(userName, deserializedEvent.CreatorName);
            Assert.AreEqual(streamName, deserializedEvent.StreamName);
            Assert.AreEqual(streamId, deserializedEvent.StreamId);
        }
    }

}
