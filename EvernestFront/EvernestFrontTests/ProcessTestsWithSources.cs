using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using EvernestFront;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFrontTests
{
 
    [TestFixture]
    public class ProcessTestsWithSources
    {
        private const string UserName = "userName";
        private const string UserName2 = "userName2";
        private const string StreamName = "streamName";
        private const string SourceName = "sourceName";
        private const string SourceName2 = "sourceName2";
        private const AccessRights SomeRight = AccessRights.ReadOnly; //constant to use when the right is not decisive

        private const string Message = "message";

        internal static String GetSourceKey_AssertSuccess(long userId, long streamId, string sourceName,
            AccessRights rights)
        {
            CreateSource ans = Process.CreateSource(userId, streamId, sourceName, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.Key);
            return ans.Key;
        }

        internal static void SetRights_AssertSuccess(String adminSource, long targetUser, AccessRights rights)
        {
            SetRights ans = Process.SetRights(adminSource, targetUser, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
        }

        internal static int GetEventId_AssertSuccess(String sourceKey, String message)
        {
            Push ans = Process.Push(sourceKey, message);
            var success = ans.Success;
            var error = ans.Error;
            var id = ans.MessageId;
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.MessageId;
        }

        [SetUp]
        public void Initialize()
        {
            UserTable.Clear();
            StreamTable.Clear();
            SourceTable.Clear();
        }


        [Test]
        public void CreateSource_Success()
        {
            var userId = ProcessTests.GetUserId_AssertSuccess(UserName);
            var streamId = ProcessTests.GetStreamId_AssertSuccess(userId,StreamName);
            var sourceKey = GetSourceKey_AssertSuccess(userId, streamId, SourceName, SomeRight);

            Source source = SourceTable.GetSource(sourceKey);
            Assert.IsNotNull(source);
            Assert.AreEqual(sourceKey, source.Key);
            Assert.AreEqual(SourceName, source.Name);

            var sourceKey2 = GetSourceKey_AssertSuccess(userId, streamId, SourceName2, SomeRight);
            Assert.AreNotEqual(sourceKey,sourceKey2);
        }

        [Test]
        public void CreateSource_SameNameDistinctUsers_Success()
        {
            var userId = ProcessTests.GetUserId_AssertSuccess(UserName);
            var streamId = ProcessTests.GetStreamId_AssertSuccess(userId, StreamName);
            var sourceKey = GetSourceKey_AssertSuccess(userId, streamId, SourceName, SomeRight);
            var userId2 = ProcessTests.GetUserId_AssertSuccess(UserName2);
            var sourceKey2 = GetSourceKey_AssertSuccess(userId2, streamId, SourceName, SomeRight);
            Assert.AreNotEqual(sourceKey,sourceKey2);
        }

        [Test]
        public void CreateSource_SourceNameTaken()
        {
            var userId = ProcessTests.GetUserId_AssertSuccess(UserName);
            var streamId = ProcessTests.GetStreamId_AssertSuccess(userId, StreamName);
            var sourceKey = GetSourceKey_AssertSuccess(userId, streamId, SourceName, AccessRights.ReadOnly);
            CreateSource ans = Process.CreateSource(userId, streamId, SourceName, AccessRights.ReadOnly);
            ProcessTests.ErrorAssert<SourceNameTaken>(ans);
        }

        [Test]
        public void SetRights_Success()
        {
            long creator = ProcessTests.GetUserId_AssertSuccess("creator");
            long reader = ProcessTests.GetUserId_AssertSuccess("reader");
            long admin = ProcessTests.GetUserId_AssertSuccess("admin");
            long stream = ProcessTests.GetStreamId_AssertSuccess(creator, StreamName);
            var creatorSource = GetSourceKey_AssertSuccess(creator, stream, SourceName, AccessRights.Admin);
            var adminSource = GetSourceKey_AssertSuccess(admin, stream, SourceName, AccessRights.Admin);
            var readerSource = GetSourceKey_AssertSuccess(reader, stream, SourceName, AccessRights.Admin);

            SetRights_AssertSuccess(creatorSource, admin, AccessRights.Admin);
            SetRights_AssertSuccess(adminSource, reader, AccessRights.ReadOnly);
            SetRights_AssertSuccess(creatorSource, reader, AccessRights.ReadWrite);
        }

        [Test]
        public void SetRights_UserCannotAdmin_AdminAccessDenied()
        {
            long creator = ProcessTests.GetUserId_AssertSuccess("creator");
            long reader = ProcessTests.GetUserId_AssertSuccess("reader");
            long stream = ProcessTests.GetStreamId_AssertSuccess(creator, StreamName);
            var readerSource = GetSourceKey_AssertSuccess(reader, stream, SourceName, AccessRights.Admin);
            ProcessTests.SetRights_AssertSuccess(creator, stream, reader, AccessRights.ReadOnly);

            SetRights ans = Process.SetRights(readerSource, reader, AccessRights.ReadWrite);
            Assert.IsFalse(ans.Success);
            ProcessTests.ErrorAssert<AdminAccessDenied>(ans);
        }

        [Test]
        public void SetRights_SourceCannotAdmin_AdminAccessDenied()
        {
            long creator = ProcessTests.GetUserId_AssertSuccess("creator");
            long target = ProcessTests.GetUserId_AssertSuccess("target");
            long stream = ProcessTests.GetStreamId_AssertSuccess(creator, StreamName);
            var source = GetSourceKey_AssertSuccess(creator, stream, SourceName, AccessRights.ReadOnly);

            SetRights ans = Process.SetRights(source, target, AccessRights.ReadWrite);
            ProcessTests.ErrorAssert<AdminAccessDenied>(ans);
        }

        [Test]
        public void SetRights_CannotDestituteAdmin()
        {
            long creator = ProcessTests.GetUserId_AssertSuccess("creator");
            long stream = ProcessTests.GetStreamId_AssertSuccess(creator, StreamName);
            var source = GetSourceKey_AssertSuccess(creator, stream, SourceName, AccessRights.Admin);

            SetRights ans = Process.SetRights(source, creator, AccessRights.NoRights);
            ProcessTests.ErrorAssert<CannotDestituteAdmin>(ans);
        }

        [Test]
        public void Push_Success()
        {
            long user = ProcessTests.GetUserId_AssertSuccess(UserName);
            long stream = ProcessTests.GetStreamId_AssertSuccess(user, StreamName);
            var source = GetSourceKey_AssertSuccess(user, stream, SourceName, AccessRights.Admin);
            int eventId = GetEventId_AssertSuccess(source, Message);
            int eventId2 = GetEventId_AssertSuccess(source, Message);
            Assert.AreEqual(eventId, 0);
            Assert.AreEqual(eventId2, 1);
        }

        [Test]
        public void Push_UserCannotWrite_WriteAccessDenied()
        {
            long creator = ProcessTests.GetUserId_AssertSuccess("creator");
            long stream = ProcessTests.GetStreamId_AssertSuccess(creator, StreamName);
            long user = ProcessTests.GetUserId_AssertSuccess(UserName);
            var source = GetSourceKey_AssertSuccess(user, stream, SourceName, AccessRights.WriteOnly);
            Push ans = Process.Push(source, Message);
            ProcessTests.ErrorAssert<WriteAccessDenied>(ans);
        }

        
    }
}
