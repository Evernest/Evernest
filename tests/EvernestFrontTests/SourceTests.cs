using System;
using EvernestFront;
using EvernestFront.Contract;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace EvernestFrontTests
{
 
    [TestFixture]
    public class SourceTests
    {
        private const AccessRight SomeRight = AccessRight.ReadOnly; //constant to use when the right is not decisive

        private const string Message = "message";

        internal static string CreateSource_GetKey_AssignStream_AssertSuccess(long userId, long streamId, string sourceName,
            AccessRight right)
        {
            UserTests.GetUser_AssertSuccess(userId);
            var key = CreateSource_GetKey_AssertSuccess(userId, sourceName);
            var sb = new SourceProvider();
            var getSource = sb.GetSource(key);
            Assert.IsTrue(getSource.Success);
            Assert.IsNull(getSource.Error);
            var source = getSource.Result;
            User user = UserTests.GetUser_AssertSuccess(userId);
            var setRight = user.SetSourceRight(source.Id, streamId, right);
            Assert.IsTrue(setRight.Success);
            Assert.IsNull(setRight.Error);
            System.Threading.Thread.Sleep(50);
            return key;
        }

        internal static string CreateSource_GetKey_AssertSuccess(long userId, string sourceName)
        {
            User user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.CreateSource(sourceName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.Result);
            var key = ans.Result.Item1;
            System.Threading.Thread.Sleep(50);
            return key;
        }

        internal static Source GetSource_AssertSuccess(string sourceKey)
        {
            var sb = new SourceProvider();
            Response<Source> ans = sb.GetSource(sourceKey);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.Result);
            return ans.Result;
        }

        internal static void SetRights_AssertSuccess(string adminSourceKey, long streamId, string targetUserName, AccessRight right)
        {
            Source adminSource = GetSource_AssertSuccess(adminSourceKey);
            var getStream = adminSource.GetEventStream(streamId);
            Assert.IsTrue(getStream.Success);
            var stream = getStream.Result;
            Response<Guid> ans;
            if (right == AccessRight.Admin)
                ans = stream.SetUserRightToAdmin(targetUserName, UserTests.Password);
            else
                ans = stream.SetUserRight(targetUserName, right);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            System.Threading.Thread.Sleep(50);
        }

        internal static long GetEventId_AssertSuccess(string sourceKey, long streamId, string message)
        {
            Source source = GetSource_AssertSuccess(sourceKey);
            var getStream = source.GetEventStream(streamId);
            Assert.IsTrue(getStream.Success);
            var stream = getStream.Result;
            Response<long> ans = stream.Push(message);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.Result;
        }
        

        [Test]
        public void GetSource_Success()
        {
            var userName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, sourceName);
            var source = GetSource_AssertSuccess(sourceKey);
            Assert.AreEqual(sourceKey, source.Key);
            Assert.AreEqual(userId, source.User.Id);
            Assert.AreEqual(sourceName, source.Name);
        }

        [Test]
        public void GetSource_SourceKeyDoesNotExist()
        {
            const string inexistantKey = "InexistantKey";
            var ans = new SourceProvider().GetSource(inexistantKey);
            Helpers.ErrorAssert(FrontError.SourceKeyDoesNotExist,ans);
        }

        [Test]
        public void SetRights_Success()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var creatorName = Helpers.NewName;
            var readerName = Helpers.NewName;
            var adminName = Helpers.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long readerId = UserTests.AddUser_GetId_AssertSuccess(readerName);
            long adminId = UserTests.AddUser_GetId_AssertSuccess(adminName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            var creatorSourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(creatorId, streamId, sourceName, AccessRight.Admin);
            var adminSourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(adminId, streamId, sourceName, AccessRight.Admin);
            var readerSourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(readerId, streamId, sourceName, AccessRight.Admin);

            SetRights_AssertSuccess(creatorSourceKey, streamId, adminName, AccessRight.Admin);
            SetRights_AssertSuccess(adminSourceKey, streamId, readerName, AccessRight.ReadOnly);
            SetRights_AssertSuccess(creatorSourceKey, streamId, readerName, AccessRight.ReadWrite);
        }

        [Test]
        public void SetRights_UserCannotAdmin_AdminAccessDenied()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var creatorName = Helpers.NewName;
            var readerName = Helpers.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long readerId = UserTests.AddUser_GetId_AssertSuccess(readerName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            var readerSourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(readerId, streamId, sourceName, AccessRight.Admin);
            EventStreamTests.SetRight_AssertSuccess(creatorId, streamId, readerName, AccessRight.ReadOnly);

            Source readerSource = GetSource_AssertSuccess(readerSourceKey);
            var ans = readerSource.User.GetEventStream(streamId).Result.SetUserRight(readerName, AccessRight.ReadWrite); //reader cannot allow himself to write
            Assert.IsFalse(ans.Success);
            Helpers.ErrorAssert(FrontError.AdminAccessDenied,ans);
        }

        [Test]
        public void SetRights_SourceCannotAdmin_AdminAccessDenied()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var creatorName = Helpers.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            string targetName = Helpers.NewName;
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(creatorId, streamId, sourceName, AccessRight.ReadOnly); //source belongs to an admin but only has reading right

            Source source = GetSource_AssertSuccess(sourceKey);
            var ans = source.GetEventStream(streamId).Result.SetUserRight(targetName, AccessRight.ReadWrite);
            Helpers.ErrorAssert(FrontError.AdminAccessDenied,ans);
        }

        [Test]
        public void SetRights_CannotDestituteAdmin()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var creatorName = Helpers.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(creatorId, streamId, sourceName, AccessRight.Admin);

            Source source = GetSource_AssertSuccess(sourceKey);
            var ans = source.GetEventStream(streamId).Result.SetUserRight(creatorName, AccessRight.NoRight);
            Helpers.ErrorAssert(FrontError.CannotDestituteAdmin,ans);
        }

        [Test]
        public void Push_Success()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var userName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(userId, streamName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName, AccessRight.Admin);
            long eventId = GetEventId_AssertSuccess(sourceKey, streamId, Message);
            long eventId2 = GetEventId_AssertSuccess(sourceKey, streamId, Message);
            Assert.AreEqual(eventId+1, eventId2);
        }

        [Test]
        public void Push_UserCannotWrite_WriteAccessDenied()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var creatorName = Helpers.NewName;
            var userName = Helpers.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName, AccessRight.WriteOnly); //user has no right on stream
            Source source = GetSource_AssertSuccess(sourceKey);
            Response<long> ans = source.GetEventStream(streamId).Result.Push(Message);
            Helpers.ErrorAssert(FrontError.WriteAccessDenied,ans);
        }


        [Test]
        public void Push_SourceCannotWrite_WriteAccessDenied()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var userName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(userId, streamName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName, AccessRight.ReadOnly); //source cannot write
            Source source = GetSource_AssertSuccess(sourceKey);
            var ans = source.GetEventStream(streamId).Result.Push(Message);
            Helpers.ErrorAssert(FrontError.WriteAccessDenied,ans);
        }



        [Test]
        public void Pull_Success()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var userName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(userId, streamName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName, AccessRight.ReadOnly);
            Source source = GetSource_AssertSuccess(sourceKey);
            long eventId = EventStreamTests.PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            var ans = source.GetEventStream(streamId).Result.Pull(eventId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            var eventPulled = ans.Result;
            Assert.AreEqual(eventPulled.Message, Message);
            Assert.AreEqual(userId, eventPulled.AuthorId);
            Assert.AreEqual(userName, eventPulled.AuthorName);
        }

        [Test]
        public void Pull_UserCannotRead_ReadAccessDenied()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var creatorName = Helpers.NewName;
            var userName = Helpers.NewName;
            long creatorId = UserTests.AddUser_GetId_AssertSuccess(creatorName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(creatorId, streamName);
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName, AccessRight.ReadOnly); //user has no right on stream
            Source source = GetSource_AssertSuccess(sourceKey);
            long eventId = EventStreamTests.PushEvent_GetId_AssertSuccess(creatorId, streamId, Message);
            var ans = source.GetEventStream(streamId).Result.Pull(eventId);
            Helpers.ErrorAssert(FrontError.ReadAccessDenied,ans);
        }

        [Test]
        public void Pull_SourceCannotRead_ReadAccessDenied()
        {
            var streamName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var userName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(userId, streamName);
            var sourceKey = CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName, AccessRight.WriteOnly); //source cannot read
            Source source = GetSource_AssertSuccess(sourceKey);
            long eventId = EventStreamTests.PushEvent_GetId_AssertSuccess(userId, streamId, Message);
            var ans = source.GetEventStream(streamId).Result.Pull(eventId);
            Helpers.ErrorAssert(FrontError.ReadAccessDenied, ans);
        }

        
    }
}
