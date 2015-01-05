using System;
using EvernestFront;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFrontTests
{

    [TestFixture]
    public class UserTestsTowardEventStream
    {
        private const string UserName = "userName";
        private const string UserName2 = "userName2";
        private const string StreamName = "streamName";
        private const string Message = "message";

        [SetUp]
        public void ResetTables()
        {
            Setup.ClearAsc();

            Projection.Clear();
        }

        internal static long CreateEventStream_GetId_AssertSuccess(long userId, string streamName)
        {
            User user = UserTests.GetUser_AssertSuccess(userId);
            CreateEventStream ans = user.CreateEventStream(streamName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.StreamId;
        }


        internal static void SetRights_AssertSuccess(long userId, long streamId, long targetUserId, AccessRights rights)
        {
            var user = UserTests.GetUser_AssertSuccess(userId);
            SetRights ans = user.SetRights(streamId, targetUserId, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
        }

        internal static long PushEvent_GetId_AssertSuccess(long userId, long streamId, string message)
        {
            var user = UserTests.GetUser_AssertSuccess(userId);
            Push ans = user.Push(streamId, message);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.MessageId;
        }

        [Test]
        public void CreateStream_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
        }


        [Test]
        public void CreateStream_StreamNameTaken()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long user2Id = UserTests.AddUser_GetId_AssertSuccess(UserName2);

            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            User user2 = UserTests.GetUser_AssertSuccess(user2Id);
            CreateEventStream ans = user2.CreateEventStream(StreamName);
            AssertAuxiliaries.ErrorAssert<EventStreamNameTaken>(ans);
        }


        [Test]
        public void SetRights_Success()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long readerId = UserTests.AddUser_GetId_AssertSuccess("reader");
            long adminId = UserTests.AddUser_GetId_AssertSuccess("admin");

            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            SetRights_AssertSuccess(creatorId, streamId, adminId, AccessRights.Admin);
            SetRights_AssertSuccess(adminId, streamId, readerId, AccessRights.ReadOnly);
            SetRights_AssertSuccess(creatorId, streamId, readerId, AccessRights.ReadWrite);
        }

        [Test]
        public void SetRights_AdminAccessDenied()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long readerId = UserTests.AddUser_GetId_AssertSuccess("reader");
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            SetRights_AssertSuccess(creatorId, streamId, readerId, AccessRights.ReadOnly);

            User reader = UserTests.GetUser_AssertSuccess(readerId);
            SetRights ans = reader.SetRights(streamId, readerId, AccessRights.ReadWrite);
            AssertAuxiliaries.ErrorAssert<AdminAccessDenied>(ans);
        }

        [Test]
        public void SetRights_CannotDestituteAdmin()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long evilAdminId = UserTests.AddUser_GetId_AssertSuccess("evilAdmin");
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            SetRights_AssertSuccess(creatorId, streamId, evilAdminId, AccessRights.Admin);

            User evilAdmin = UserTests.GetUser_AssertSuccess(evilAdminId);
            SetRights ans = evilAdmin.SetRights(streamId, creatorId, AccessRights.NoRights);
            AssertAuxiliaries.ErrorAssert<CannotDestituteAdmin>(ans);
        }

        [Test]
        public void Push_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, "Push_Success");
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            long eventId2 = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            //Assert.AreEqual(0, eventId);
            //Assert.AreEqual(1, eventId2); //works if .txt dump files are clean : TODO update when stream recuperation/creation is fixed

        }

        [Test]
        public void Push_WriteAccessDenied()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);

            long user2Id = UserTests.AddUser_GetId_AssertSuccess(UserName2);
            User user2 = UserTests.GetUser_AssertSuccess(user2Id);
            Push ans = user2.Push(streamId, Message);
            AssertAuxiliaries.ErrorAssert<WriteAccessDenied>(ans);
        }

        [Test]
        public void Push_StreamIdDoesNotExist()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            const long streamId = 42; //does not exist in StreamTable
            User user = UserTests.GetUser_AssertSuccess(userId);
            Push ans = user.Push(streamId, Message);
            AssertAuxiliaries.ErrorAssert<EventStreamIdDoesNotExist>(ans);
        }

        [Test]
        public void PullRandom_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, "PullRandom_Success");
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            User user = UserTests.GetUser_AssertSuccess(userId);
            PullRandom ans = user.PullRandom(streamId);
            Assert.IsTrue(ans.Success);
            Event pulledRandom = ans.EventPulled;
            Assert.IsNotNull(pulledRandom);
            //Assert.AreEqual(eventId, pulledRandom.Id);
            //Assert.AreEqual(pulledRandom.Message, Message); //will work when stream recuperation/creation is fixed : TODO 
        }

        [Test]
        public void Pull_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            User user = UserTests.GetUser_AssertSuccess(userId);
            Pull ans = user.Pull(streamId, eventId);
            Assert.IsTrue(ans.Success);
            Event pulledById = ans.EventPulled;
            Assert.IsNotNull(pulledById);
            Assert.AreEqual(pulledById.Message, Message); //will work when we connect to back-end : TODO
        }

        [Test]
        public void PullRange_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            long eventId2 = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            User user = UserTests.GetUser_AssertSuccess(userId);
            PullRange ans = user.PullRange(streamId, eventId, eventId2);
            Assert.IsTrue(ans.Success);
            var pulled = ans.Events;
            Assert.AreEqual(pulled.Count, 2);
        }

        [Test]
        public void Pull_ReadAccessDenied()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            long user2Id = UserTests.AddUser_GetId_AssertSuccess(UserName2);
            SetRights_AssertSuccess(userId, streamId, user2Id, AccessRights.WriteOnly);

            User user2 = UserTests.GetUser_AssertSuccess(user2Id);
            Pull ans = user2.Pull(streamId, eventId);
            AssertAuxiliaries.ErrorAssert<ReadAccessDenied>(ans);
        }

        [Test]
        public void Pull_InvalidEventId()
        {
            const long invalidEventId = 42;
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.Pull(streamId, invalidEventId);
            AssertAuxiliaries.ErrorAssert<InvalidEventId>(ans);
            Assert.AreEqual(streamId, (ans.Error as InvalidEventId).StreamId);
            Assert.AreEqual(invalidEventId, (ans.Error as InvalidEventId).EventId);
        }
    }
}
