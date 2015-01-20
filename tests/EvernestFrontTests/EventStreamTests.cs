using System.Collections.Generic;
using System.Threading;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvernestFront.Contract;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace EvernestFrontTests
{   
    [TestFixture]
    class EventStreamTests
    {
        private const string Message = "Message";

        [SetUp]
        public void Initialize()
        {
            //TODO : clear tables ?
            Setup.ClearAsc();
        }

        internal static long CreateEventStream_GetId_AssertSuccess(long userId, string streamName)
        {
            User user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.CreateEventStream(streamName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            //the creation of a pageblob is a bit long (syn+alloc+ack at least), so i extended the delay
            //otherwise the test don't pass since the stream isn't created yet when you request it
            //ok, it's AWFULLY long, but hey, you're not supposed to create pageblobs everyday!
            Thread.Sleep(5000);
            var get = user.GetEventStream(streamName);
            Assert.IsTrue(get.Success);
            return get.Result.Id;
        }


        internal static void SetRights_AssertSuccess(long userId, long streamId, string targetUserName, AccessRight rights)
        {
            var user = UserTests.GetUser_AssertSuccess(userId);
            var getStream = user.GetEventStream(streamId);
            var stream = getStream.Result;
            var ans = stream.SetUserRight(targetUserName, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Thread.Sleep(100);
        }

        internal static long PushEvent_GetId_AssertSuccess(long userId, long streamId, string message)
        {
            var user = UserTests.GetUser_AssertSuccess(userId);
            var getStream = user.GetEventStream(streamId);
            Assert.IsTrue(getStream.Success);
            Assert.IsNull(getStream.Error);
            var stream = getStream.Result;
            Response<long> ans = stream.Push(message);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.Result;
        }
        

        [Test]
        public void GetEventStream()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(AssertAuxiliaries.NewName);
            string streamName = AssertAuxiliaries.NewName;
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.GetEventStream(streamId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            var stream = ans.Result;
            Assert.AreEqual(streamId, stream.Id);
            Assert.AreEqual(streamName, stream.Name);
            Assert.AreEqual(0, stream.Count);
        }

        [Test]
        public void GetEventStream_IdDoesNotExist()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(AssertAuxiliaries.NewName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            const long streamId = 3213554622; //does not exist
            var ans = user.GetEventStream(streamId);
            AssertAuxiliaries.ErrorAssert(FrontError.EventStreamIdDoesNotExist, ans);
        }
        

        [Test]
        public void CreateStream_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
        }


        [Test]
        public void CreateStream_StreamNameTaken()
        {
            var userName = AssertAuxiliaries.NewName;
            var userName2 = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long user2Id = UserTests.AddUser_GetId_AssertSuccess(userName2);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            User user2 = UserTests.GetUser_AssertSuccess(user2Id);
            var ans = user2.CreateEventStream(streamName);
            AssertAuxiliaries.ErrorAssert(FrontError.EventStreamNameTaken, ans);
        }


        [Test]
        public void SetUserRights_Success()
        {
            var creatorName = AssertAuxiliaries.NewName;
            var readerName = AssertAuxiliaries.NewName;
            var adminName = AssertAuxiliaries.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long readerId = UserTests.AddUser_GetId_AssertSuccess(readerName);
            long adminId = UserTests.AddUser_GetId_AssertSuccess(adminName);
            var streamName = AssertAuxiliaries.NewName;
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            SetRights_AssertSuccess(creatorId, streamId, adminName, AccessRight.Admin);
            SetRights_AssertSuccess(adminId, streamId, readerName, AccessRight.ReadOnly);
            SetRights_AssertSuccess(creatorId, streamId, readerName, AccessRight.ReadWrite);
        }

        [Test]
        public void SetUserRightsById_Success()
        {
            var creatorName = AssertAuxiliaries.NewName;
            var adminName = AssertAuxiliaries.NewName;
            var readerName = AssertAuxiliaries.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long adminId = UserTests.AddUser_GetId_AssertSuccess(adminName);
            long readerId = UserTests.AddUser_GetId_AssertSuccess(readerName);
            var streamName = AssertAuxiliaries.NewName;
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            
            var creatorUser = UserTests.GetUser_AssertSuccess(creatorId);
            var getStream = creatorUser.GetEventStream(streamId);
            var stream = getStream.Result;
            var ans = stream.SetUserRight(adminId, AccessRight.Admin);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Thread.Sleep(100);

            var adminUser = UserTests.GetUser_AssertSuccess(adminId);
            getStream = adminUser.GetEventStream(streamId);
            stream = getStream.Result;
            ans = stream.SetUserRight(readerId, AccessRight.ReadOnly);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
        }

        [Test]
        public void SetUserRights_AdminAccessDenied()
        {
            var streamName = AssertAuxiliaries.NewName;
            var creatorName = AssertAuxiliaries.NewName;
            var readerName = AssertAuxiliaries.NewName;
            var adminName = AssertAuxiliaries.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long readerId = UserTests.AddUser_GetId_AssertSuccess(readerName);
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            SetRights_AssertSuccess(creatorId, streamId, readerName, AccessRight.ReadOnly);

            User reader = UserTests.GetUser_AssertSuccess(readerId);
            var getReaderStream = reader.GetEventStream(streamName);
            var readerStream = getReaderStream.Result;
            var ans = readerStream.SetUserRight(readerName, AccessRight.ReadWrite);
            AssertAuxiliaries.ErrorAssert(FrontError.AdminAccessDenied, ans);
        }

        [Test]
        public void SetUserRights_CannotDestituteAdmin()
        {
            var streamName = AssertAuxiliaries.NewName;
            var creatorName = AssertAuxiliaries.NewName;
            var evilAdminName = AssertAuxiliaries.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long evilAdminId = UserTests.AddUser_GetId_AssertSuccess(evilAdminName);
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            SetRights_AssertSuccess(creatorId, streamId, evilAdminName, AccessRight.Admin);

            User evilAdmin = UserTests.GetUser_AssertSuccess(evilAdminId);
            var getEvilAdminStream = evilAdmin.GetEventStream(streamName);
            var evilAdminStream = getEvilAdminStream.Result;
            var ans = evilAdminStream.SetUserRight(creatorName, AccessRight.NoRight);
            AssertAuxiliaries.ErrorAssert(FrontError.CannotDestituteAdmin, ans);
        }

        [Test]
        public void SetUserRightsById_UserIdDoesNotExist()
        {
            var creatorName = AssertAuxiliaries.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long inexistantId = long.MaxValue;
            var streamName = AssertAuxiliaries.NewName;
            long streamId = CreateEventStream_GetId_AssertSuccess(creatorId, streamName);

            var creatorUser = UserTests.GetUser_AssertSuccess(creatorId);
            var getStream = creatorUser.GetEventStream(streamId);
            var stream = getStream.Result;
            var ans = stream.SetUserRight(inexistantId, AccessRight.Admin);
            Assert.IsFalse(ans.Success);
            Assert.AreEqual(FrontError.UserIdDoesNotExist, ans.Error);
        }

        [Test]
        public void Push_Success()
        {
            var streamName = AssertAuxiliaries.NewName;
            var userName = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            long eventId2 = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            Assert.AreEqual(0, eventId);
            Assert.AreEqual(1, eventId2);

        }

        [Test]
        public void Push_WriteAccessDenied()
        {
            var streamName = AssertAuxiliaries.NewName;
            var userName = AssertAuxiliaries.NewName;
            var userName2 = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);

            long user2Id = UserTests.AddUser_GetId_AssertSuccess(userName2);
            User user2 = UserTests.GetUser_AssertSuccess(user2Id);
            var getStream = user2.GetEventStream(streamName);
            Assert.IsTrue(getStream.Success);
            var stream = getStream.Result;
            Response<long> ans = stream.Push(Message);
            AssertAuxiliaries.ErrorAssert(FrontError.WriteAccessDenied, ans);
        }

        [Test]
        public void PullRandom_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var getStream = user.GetEventStream(streamName);
            Assert.IsTrue(getStream.Success);
            var stream = getStream.Result;
            Response<Event> ans = stream.PullRandom();
            Assert.IsTrue(ans.Success);
            Event pulledRandom = ans.Result;
            Assert.IsNotNull(pulledRandom);
            Assert.AreEqual(eventId, pulledRandom.Id); //works because there is only one event in the stream
            Assert.AreEqual(pulledRandom.Message, Message); 
        }

        [Test]
        public void Pull_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var stream = user.GetEventStream(streamName).Result;
            Response<Event> ans = stream.Pull(eventId);
            Assert.IsTrue(ans.Success);
            Event pulledById = ans.Result;
            Assert.IsNotNull(pulledById);
            Assert.AreEqual(pulledById.Message, Message); //works because there is only one event in the stream
        }

        [Test]
        public void PullRange_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            long eventId2 = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var stream = user.GetEventStream(streamName).Result;
            Response<List<Event>> ans = stream.PullRange(eventId, eventId2);
            Assert.IsTrue(ans.Success);
            var pulled = ans.Result;
            Assert.AreEqual(pulled.Count, 2);
        }

        [Test]
        public void Pull_ReadAccessDenied()
        {
            var userName = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            var userName2 = AssertAuxiliaries.NewName;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            long eventId = PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            long user2Id = UserTests.AddUser_GetId_AssertSuccess(userName2);
            SetRights_AssertSuccess(userId, streamId, userName2, AccessRight.WriteOnly);

            User user2 = UserTests.GetUser_AssertSuccess(user2Id);
            var stream = user2.GetEventStream(streamName).Result;
            Response<Event> ans = stream.Pull(eventId);
            AssertAuxiliaries.ErrorAssert(FrontError.ReadAccessDenied, ans);
        }

        [Test]
        public void Pull_InvalidEventId()
        {
            var userName = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            const long invalidEventId = 42;
            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, streamName);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var stream = user.GetEventStream(streamName).Result;
            var ans = stream.Pull(invalidEventId);
            AssertAuxiliaries.ErrorAssert(FrontError.InvalidEventId, ans);
        }
    }
}
