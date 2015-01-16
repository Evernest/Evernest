using EvernestFront;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;

namespace EvernestFrontTests
{
 
    [TestFixture]
    public class SourceTests
    {
        private const string UserName = "userName";
        private const string UserName2 = "userName2";
        private const string StreamName = "streamName";
        private const string SourceName = "sourceName";
        private const string SourceName2 = "sourceName2";
        private const AccessRights SomeRight = AccessRights.ReadOnly; //constant to use when the right is not decisive

        private const string Message = "message";

        internal static string CreateSource_GetKey_AssertSuccess(long userId, long streamId, string sourceName,
            AccessRights rights)
        {
            User user = UserTests.GetUser_AssertSuccess(userId);
            CreateSource ans = user.CreateSource(sourceName, streamId, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.Key);
            return ans.Key;
        }

        internal static Source GetSource_AssertSuccess(string sourceKey)
        {
            GetSource ans = Source.GetSource(sourceKey);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.Source);
            return ans.Source;
        }

        internal static void SetRights_AssertSuccess(string adminSourceKey, long targetUserId, AccessRights rights)
        {
            Source adminSource = GetSource_AssertSuccess(adminSourceKey);
            SetRights ans = adminSource.SetRights(targetUserId, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
        }

        internal static long GetEventId_AssertSuccess(string sourceKey, string message)
        {
            Source source = GetSource_AssertSuccess(sourceKey);
            Push ans = source.Push(message);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.MessageId;
        }

        [SetUp]
        public void ClearProjection()
        {
            //TODO : clear tables ?
            Setup.ClearAsc();
        }

        [Test]
        public void CreateSource_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId,StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName, SomeRight);

            var source2Key = CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName2, SomeRight);
            Assert.AreNotEqual(sourceKey,source2Key);
        }

        [Test]
        public void CreateSource_SameNameDistinctUsers_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName, SomeRight);
            var user2Id = UserTests.AddUser_GetId_AssertSuccess(UserName2);
            var source2Key = CreateSource_GetKey_AssertSuccess(user2Id, streamId, SourceName, SomeRight);
            Assert.AreNotEqual(sourceKey,source2Key);
        }

        [Test]
        public void CreateSource_SourceNameTaken()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var source = CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName, SomeRight);
            User user = UserTests.GetUser_AssertSuccess(userId);
            CreateSource ans = user.CreateSource(SourceName, streamId, AccessRights.ReadOnly);
            AssertAuxiliaries.ErrorAssert(FrontError.SourceNameTaken,ans);
        }

        [Test]
        public void DeleteSource_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName, SomeRight);

            var source = GetSource_AssertSuccess(sourceKey);
            DeleteSource ans = source.Delete();

            Assert.IsTrue(ans.Success);
            var user = UserTests.GetUser_AssertSuccess(userId);
            Assert.IsFalse(user.Sources.Exists(pair => pair.Key==SourceName));

        }

        [Test]
        public void GetSource_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName, SomeRight);
            var source = GetSource_AssertSuccess(sourceKey);
            Assert.AreEqual(sourceKey, source.Key);
            Assert.AreEqual(userId, source.User.Id);
            Assert.AreEqual(streamId, source.EventStream.Id);
            Assert.AreEqual(SourceName, source.Name);
            Assert.AreEqual(SomeRight, source.Right);
        }

        [Test]
        public void GetSource_SourceKeyDoesNotExist()
        {
            const string inexistantKey = "InexistantKey";
            var ans = Source.GetSource(inexistantKey);
            AssertAuxiliaries.ErrorAssert(FrontError.SourceKeyDoesNotExist,ans);
        }

        [Test]
        public void SetRights_Success()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long readerId = UserTests.AddUser_GetId_AssertSuccess("reader");
            long adminId = UserTests.AddUser_GetId_AssertSuccess("admin");
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            var creatorSourceKey = CreateSource_GetKey_AssertSuccess(creatorId, stream, SourceName, AccessRights.Admin);
            var adminSourceKey = CreateSource_GetKey_AssertSuccess(adminId, stream, SourceName, AccessRights.Admin);
            var readerSourceKey = CreateSource_GetKey_AssertSuccess(readerId, stream, SourceName, AccessRights.Admin);

            SetRights_AssertSuccess(creatorSourceKey, adminId, AccessRights.Admin);
            SetRights_AssertSuccess(adminSourceKey, readerId, AccessRights.ReadOnly);
            SetRights_AssertSuccess(creatorSourceKey, readerId, AccessRights.ReadWrite);
        }

        [Test]
        public void SetRights_UserCannotAdmin_AdminAccessDenied()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long readerId = UserTests.AddUser_GetId_AssertSuccess("reader");
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            var readerSourceKey = CreateSource_GetKey_AssertSuccess(readerId, stream, SourceName, AccessRights.Admin);
            UserTestsTowardEventStream.SetRights_AssertSuccess(creatorId, stream, readerId, AccessRights.ReadOnly);

            Source readerSource = GetSource_AssertSuccess(readerSourceKey);
            SetRights ans = readerSource.SetRights(readerId, AccessRights.ReadWrite); //reader cannot allow himself to write
            Assert.IsFalse(ans.Success);
            AssertAuxiliaries.ErrorAssert(FrontError.AdminAccessDenied,ans);
        }

        [Test]
        public void SetRights_SourceCannotAdmin_AdminAccessDenied()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long targetId = UserTests.AddUser_GetId_AssertSuccess("target");
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(creatorId, stream, SourceName, AccessRights.ReadOnly); //source belongs to an admin but only had reading rights

            Source source = GetSource_AssertSuccess(sourceKey);
            SetRights ans = source.SetRights(targetId, AccessRights.ReadWrite);
            AssertAuxiliaries.ErrorAssert(FrontError.AdminAccessDenied,ans);
        }

        [Test]
        public void SetRights_CannotDestituteAdmin()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(creatorId, stream, SourceName, AccessRights.Admin);

            Source source = GetSource_AssertSuccess(sourceKey);
            SetRights ans = source.SetRights(creatorId, AccessRights.NoRights);
            AssertAuxiliaries.ErrorAssert(FrontError.CannotDestituteAdmin,ans);
        }

        [Test]
        public void Push_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, stream, SourceName, AccessRights.Admin);
            long eventId = GetEventId_AssertSuccess(sourceKey, Message);
            long eventId2 = GetEventId_AssertSuccess(sourceKey, Message);
            Assert.AreEqual(eventId+1, eventId2);
        }

        [Test]
        public void Push_UserCannotWrite_WriteAccessDenied()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, stream, SourceName, AccessRights.WriteOnly); //user has no rights on stream
            Source source = GetSource_AssertSuccess(sourceKey);
            Push ans = source.Push(Message);
            AssertAuxiliaries.ErrorAssert(FrontError.WriteAccessDenied,ans);
        }


        [Test]
        public void Push_SourceCannotWrite_WriteAccessDenied()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, stream, SourceName, AccessRights.ReadOnly); //source cannot write
            Source source = GetSource_AssertSuccess(sourceKey);
            var ans = source.Push(Message);
            AssertAuxiliaries.ErrorAssert(FrontError.WriteAccessDenied,ans);
        }



        [Test]
        public void Pull_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, stream, SourceName, AccessRights.ReadOnly);
            Source source = GetSource_AssertSuccess(sourceKey);
            long eventId = UserTestsTowardEventStream.PushEvent_GetId_AssertSuccess(userId, stream, Message);
            var ans = source.Pull(eventId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            var eventPulled = ans.EventPulled;
            Assert.AreEqual(eventPulled.Message, Message);
            Assert.AreEqual(userId, eventPulled.AuthorId);
            Assert.AreEqual(UserName, eventPulled.AuthorName);
        }

        [Test]
        public void Pull_UserCannotRead_ReadAccessDenied()
        {
            long creatorId = UserTests.AddUser_GetId_AssertSuccess("creator");
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(creatorId, StreamName);
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, stream, SourceName, AccessRights.ReadOnly); //user has no rights on stream
            Source source = GetSource_AssertSuccess(sourceKey);
            long eventId = UserTestsTowardEventStream.PushEvent_GetId_AssertSuccess(creatorId, stream, Message);
            var ans = source.Pull(eventId);
            AssertAuxiliaries.ErrorAssert(FrontError.ReadAccessDenied,ans);
        }

        [Test]
        public void Pull_SourceCannotRead_ReadAccessDenied()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long stream = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = CreateSource_GetKey_AssertSuccess(userId, stream, SourceName, AccessRights.WriteOnly); //source cannot read
            Source source = GetSource_AssertSuccess(sourceKey);
            long eventId = UserTestsTowardEventStream.PushEvent_GetId_AssertSuccess(userId, stream, Message);
            var ans = source.Pull(eventId);
            AssertAuxiliaries.ErrorAssert(FrontError.ReadAccessDenied,ans);
        }

        
    }
}
