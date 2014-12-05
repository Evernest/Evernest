using System;
using EvernestFront;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFrontTests
{

    [TestFixture]
    public partial class UserTests
    {
        private const string StreamName = "streamName";
        private const string Message = "message";

        internal static long CreateEventStream_GetId_AssertSuccess(long userId, string streamName)
        {
            User user = GetUser_AssertSuccess(userId);
            CreateEventStream ans = user.CreateEventStream(streamName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.StreamId;
        }


        internal static void SetRights_AssertSuccess(long userId, long streamId, long targetUserId, AccessRights rights)
        {
            var user = GetUser_AssertSuccess(userId);
            SetRights ans = user.SetRights(streamId, targetUserId, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
        }

        internal static int GetEventId_AssertSuccess(long userId, long streamId, String message)
        {
            var user = GetUser_AssertSuccess(userId);
            Push ans = user.Push(streamId, message);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.MessageId;
        }

        [Test]
        public void CreateStream_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = UserTests.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
        }


        [Test]
        public void CreateStream_StreamNameTaken()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long user2Id = UserTests.AddUser_GetId_AssertSuccess(UserName2);

            long streamId = UserTests.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            User user2 = UserTests.GetUser_AssertSuccess(user2Id);
            CreateEventStream ans = user2.CreateEventStream(StreamName);
            AssertAuxiliaries.ErrorAssert<EventStreamNameTaken>(ans);
        }


        [Test]
        public void SetRights_Success()
        {
            long creatorId = AddUser_GetId_AssertSuccess("creator");
            long readerId = AddUser_GetId_AssertSuccess("reader");
            long adminId = AddUser_GetId_AssertSuccess("admin");

            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            SetRights_AssertSuccess(creatorId, streamId, adminId, AccessRights.Admin);
            SetRights_AssertSuccess(adminId, streamId, readerId, AccessRights.ReadOnly);
            SetRights_AssertSuccess(creatorId, streamId, readerId, AccessRights.ReadWrite);
        }

        [Test]
        public void SetRights_AdminAccessDenied()
        {
            long creatorId = AddUser_GetId_AssertSuccess("creator");
            long readerId = AddUser_GetId_AssertSuccess("reader");
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            SetRights_AssertSuccess(creatorId, streamId, readerId, AccessRights.ReadOnly);

            User reader = GetUser_AssertSuccess(readerId);
            SetRights ans = reader.SetRights(streamId, readerId, AccessRights.ReadWrite);
            AssertAuxiliaries.ErrorAssert<AdminAccessDenied>(ans);
        }

        [Test]
        public void SetRights_CannotDestituteAdmin()
        {
            long creatorId = AddUser_GetId_AssertSuccess("creator");
            long evilAdminId = AddUser_GetId_AssertSuccess("evilAdmin");
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            SetRights_AssertSuccess(creatorId, streamId, evilAdminId, AccessRights.Admin);

            User evilAdmin = GetUser_AssertSuccess(evilAdminId);
            SetRights ans = evilAdmin.SetRights(streamId, creatorId, AccessRights.NoRights);
            AssertAuxiliaries.ErrorAssert<CannotDestituteAdmin>(ans);
        }

        [Test]
        public void Push_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            int eventId2 = GetEventId_AssertSuccess(userId, streamId, Message);
            Assert.AreEqual(eventId, 0);
            Assert.AreEqual(eventId2, 1);

        }

        [Test]
        public void Push_WriteAccessDenied()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);

            long user2Id = AddUser_GetId_AssertSuccess(UserName2);
            User user2 = GetUser_AssertSuccess(user2Id);
            Push ans = user2.Push(streamId, Message);
            AssertAuxiliaries.ErrorAssert<WriteAccessDenied>(ans);
        }

        [Test]
        public void Push_StreamIdDoesNotExist()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            const long streamId = 42; //does not exist in StreamTable
            User user = GetUser_AssertSuccess(userId);
            Push ans = user.Push(streamId, Message);
            AssertAuxiliaries.ErrorAssert<EventStreamIdDoesNotExist>(ans);
        }

        [Test]
        public void PullRandom_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            User user = GetUser_AssertSuccess(userId);
            PullRandom ans = user.PullRandom(streamId);
            Assert.IsTrue(ans.Success);
            Event pulledRandom = ans.EventPulled;
            Assert.IsNotNull(pulledRandom);
            Assert.AreEqual(eventId, pulledRandom.Id);
            Assert.AreEqual(pulledRandom.Message, Message); //will work when we connect to back-end : TODO 
        }

        [Test]
        public void Pull_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            User user = GetUser_AssertSuccess(userId);
            Pull ans = user.Pull(streamId, eventId);
            Assert.IsTrue(ans.Success);
            Event pulledById = ans.EventPulled;
            Assert.IsNotNull(pulledById);
            Assert.AreEqual(pulledById.Message, Message); //will work when we connect to back-end : TODO
        }

        [Test]
        public void PullRange_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            int eventId2 = GetEventId_AssertSuccess(userId, streamId, Message);
            User user = GetUser_AssertSuccess(userId);
            PullRange ans = user.PullRange(streamId, eventId, eventId2);
            Assert.IsTrue(ans.Success);
            var pulled = ans.Events;
            Assert.AreEqual(pulled.Count, 2);
        }

        [Test]
        public void Pull_ReadAccessDenied()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            long user2Id = AddUser_GetId_AssertSuccess(UserName2);
            SetRights_AssertSuccess(userId, streamId, user2Id, AccessRights.WriteOnly);

            User user2 = GetUser_AssertSuccess(user2Id);
            Pull ans = user2.Pull(streamId, eventId);
            AssertAuxiliaries.ErrorAssert<ReadAccessDenied>(ans);
        }
    }
}
