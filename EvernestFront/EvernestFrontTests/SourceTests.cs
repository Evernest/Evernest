using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using EvernestFront;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

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

        internal static Source GetSource_AssertSuccess(User user, long streamId, string sourceName,
            AccessRights rights)
        {
            CreateSource ans = user.CreateSource(sourceName, streamId, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.Key);
            GetSource getSourceAns = Source.GetSource(ans.Key);
            Assert.IsTrue(getSourceAns.Success);
            Assert.IsNull(getSourceAns.Error);
            Assert.IsNotNull(getSourceAns.Source);
            return getSourceAns.Source;
        }

        internal static void SetRights_AssertSuccess(Source adminSource, long targetUser, AccessRights rights)
        {
            SetRights ans = adminSource.SetRights(targetUser, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
        }

        internal static int GetEventId_AssertSuccess(Source source, String message)
        {
            Push ans = source.Push(message);
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
        [SetUp]
        public void ClearProjection()
        {
            Projection.Clear();
        }

        [Test]
        public void CreateSource_Success()
        {
            var user = UserTests.GetUser_AssertSuccess(UserName);
            var streamId = StreamTests.GetStreamId_AssertSuccess(user.Id,StreamName);
            var source = GetSource_AssertSuccess(user, streamId, SourceName, SomeRight);

            var source2 = GetSource_AssertSuccess(user, streamId, SourceName2, SomeRight);
            Assert.AreNotEqual(source.Key,source2.Key);
        }

        [Test]
        public void CreateSource_SameNameDistinctUsers_Success()
        {
            var user = UserTests.GetUser_AssertSuccess(UserName);
            var streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            var source = GetSource_AssertSuccess(user, streamId, SourceName, SomeRight);
            var user2 = UserTests.GetUser_AssertSuccess(UserName2);
            var source2 = GetSource_AssertSuccess(user2, streamId, SourceName, SomeRight);
            Assert.AreNotEqual(source.Key,source2.Key);
        }

        [Test]
        public void CreateSource_SourceNameTaken()
        {
            var user = UserTests.GetUser_AssertSuccess(UserName);
            var streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            var source = GetSource_AssertSuccess(user, streamId, SourceName, SomeRight);
            CreateSource ans = user.CreateSource(SourceName, streamId, AccessRights.ReadOnly);
            ProcessTests.ErrorAssert<SourceNameTaken>(ans);
        }

        [Test]
        public void SetRights_Success()
        {
            User creator = UserTests.GetUser_AssertSuccess("creator");
            User reader = UserTests.GetUser_AssertSuccess("reader");
            User admin = UserTests.GetUser_AssertSuccess("admin");
            long stream = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            var creatorSource = GetSource_AssertSuccess(creator, stream, SourceName, AccessRights.Admin);
            var adminSource = GetSource_AssertSuccess(admin, stream, SourceName, AccessRights.Admin);
            var readerSource = GetSource_AssertSuccess(reader, stream, SourceName, AccessRights.Admin);

            SetRights_AssertSuccess(creatorSource, admin.Id, AccessRights.Admin);
            SetRights_AssertSuccess(adminSource, reader.Id, AccessRights.ReadOnly);
            SetRights_AssertSuccess(creatorSource, reader.Id, AccessRights.ReadWrite);
        }

        [Test]
        public void SetRights_UserCannotAdmin_AdminAccessDenied()
        {
            User creator = UserTests.GetUser_AssertSuccess("creator");
            User reader = UserTests.GetUser_AssertSuccess("reader");
            long stream = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            var readerSource = GetSource_AssertSuccess(reader, stream, SourceName, AccessRights.Admin);
            UserTests.SetRights_AssertSuccess(creator, stream, reader.Id, AccessRights.ReadOnly);

            SetRights ans = readerSource.SetRights(reader.Id, AccessRights.ReadWrite); //reader cannot allow himself to write
            Assert.IsFalse(ans.Success);
            ProcessTests.ErrorAssert<AdminAccessDenied>(ans);
        }

        [Test]
        public void SetRights_SourceCannotAdmin_AdminAccessDenied()
        {
            User creator = UserTests.GetUser_AssertSuccess("creator");
            User target = UserTests.GetUser_AssertSuccess("target");
            long stream = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            var source = GetSource_AssertSuccess(creator, stream, SourceName, AccessRights.ReadOnly); //source belongs to an admin but only had reading rights

            SetRights ans = source.SetRights(target.Id, AccessRights.ReadWrite);
            ProcessTests.ErrorAssert<AdminAccessDenied>(ans);
        }

        [Test]
        public void SetRights_CannotDestituteAdmin()
        {
            User creator = UserTests.GetUser_AssertSuccess("creator");
            long stream = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            var source = GetSource_AssertSuccess(creator, stream, SourceName, AccessRights.Admin);

            SetRights ans = source.SetRights(creator.Id, AccessRights.NoRights);
            ProcessTests.ErrorAssert<CannotDestituteAdmin>(ans);
        }

        [Test]
        public void Push_Success()
        {
            User user = UserTests.GetUser_AssertSuccess(UserName);
            long stream = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            var source = GetSource_AssertSuccess(user, stream, SourceName, AccessRights.Admin);
            int eventId = GetEventId_AssertSuccess(source, Message);
            int eventId2 = GetEventId_AssertSuccess(source, Message);
            Assert.AreEqual(eventId, 0);
            Assert.AreEqual(eventId2, 1);
        }

        [Test]
        public void Push_UserCannotWrite_WriteAccessDenied()
        {
            User creator = UserTests.GetUser_AssertSuccess("creator");
            long stream = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            User user = UserTests.GetUser_AssertSuccess(UserName);
            var source = GetSource_AssertSuccess(user, stream, SourceName, AccessRights.WriteOnly); //user has no rights on stream
            Push ans = source.Push(Message);
            ProcessTests.ErrorAssert<WriteAccessDenied>(ans);
        }

        
    }
}
