using System;
using EvernestFront;
using EvernestFront.Exceptions;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

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
            var userId = Process.AddUser(UserName);
            var streamId = Process.CreateStream(userId,StreamName);
            var sourceKey = Process.CreateSource(userId, streamId, SourceName, SomeRight);

            Source source = SourceTable.GetSource(sourceKey);
            Assert.IsNotNull(source);
            Assert.AreEqual(sourceKey, source.Key);
            Assert.AreEqual(SourceName, source.Name);

            var sourceKey2 = Process.CreateSource(userId, streamId, SourceName2, SomeRight);
            Assert.AreNotEqual(sourceKey,sourceKey2);
        }

        [Test]
        public void CreateSource_SameNameDistinctUsers_Success()
        {
            var userId = Process.AddUser(UserName);
            var streamId = Process.CreateStream(userId, StreamName);
            var sourceKey = Process.CreateSource(userId, streamId, SourceName, SomeRight);
            var userId2 = Process.AddUser(UserName2);
            var sourceKey2 = Process.CreateSource(userId2, streamId, SourceName, SomeRight);
        }

        [Test]
        [ExpectedException(typeof(SourceNameTakenException))]
        public void CreateSource_SourceNameTaken()
        {
            var userId = Process.AddUser(UserName);
            var streamId = Process.CreateStream(userId, StreamName);
            var sourceKey = Process.CreateSource(userId, streamId, SourceName, AccessRights.ReadOnly);
            var sourceKey2 = Process.CreateSource(userId, streamId, SourceName, AccessRights.ReadOnly);
        }

        [Test]
        public void SetRights_Success()
        {
            long creator = Process.AddUser("creator");
            long reader = Process.AddUser("reader");
            long admin = Process.AddUser("admin");
            long stream = Process.CreateStream(creator, StreamName);
            var creatorSource = Process.CreateSource(creator, stream, SourceName, AccessRights.Admin);
            var adminSource = Process.CreateSource(admin, stream, SourceName, AccessRights.Admin);
            var readerSource = Process.CreateSource(reader, stream, SourceName, AccessRights.Admin);

            Process.SetRights(creatorSource, admin, AccessRights.Admin);
            Process.SetRights(adminSource, reader, AccessRights.ReadOnly);
            Process.SetRights(creatorSource, reader, AccessRights.ReadWrite);
        }

        [Test]
        [ExpectedException(typeof(AdminAccessDeniedException))]
        public void SetRights_UserCannotAdmin_AdminAccessDenied()
        {
            long creator = Process.AddUser("creator");
            long reader = Process.AddUser("reader");
            long stream = Process.CreateStream(creator, StreamName);
            var readerSource = Process.CreateSource(reader, stream, SourceName, AccessRights.Admin);
            Process.SetRights(creator, stream, reader, AccessRights.ReadOnly);

            Process.SetRights(readerSource, reader, AccessRights.ReadWrite);
        }

        [Test]
        [ExpectedException(typeof(AdminAccessDeniedException))]
        public void SetRights_SourceCannotAdmin_AdminAccessDenied()
        {
            long creator = Process.AddUser("creator");
            long target = Process.AddUser("target");
            long stream = Process.CreateStream(creator, StreamName);
            var source = Process.CreateSource(creator, stream, SourceName, AccessRights.ReadOnly);

            Process.SetRights(source, target, AccessRights.ReadWrite);
        }

        [Test]
        [ExpectedException(typeof(CannotDestituteAdminException))]
        public void SetRights_CannotDestituteAdmin()
        {
            long creator = Process.AddUser("creator");
            long stream = Process.CreateStream(creator, StreamName);
            var source = Process.CreateSource(creator, stream, SourceName, AccessRights.Admin);

            Process.SetRights(source, creator, AccessRights.NoRights);
        }

        [Test]
        public void Push_Success()
        {
            long user = Process.AddUser(UserName);
            long stream = Process.CreateStream(user, StreamName);
            var source = Process.CreateSource(user, stream, SourceName, AccessRights.Admin);
            int eventId = Process.Push(source, Message);
            int eventId2 = Process.Push(source, Message);
            Assert.AreEqual(eventId, 0);
            Assert.AreEqual(eventId2, 1);
        }

        [Test]
        [ExpectedException(typeof(WriteAccessDeniedException))]
        public void Push_UserCannotWrite_WriteAccessDenied()
        {
            long creator = Process.AddUser("creator");
            long stream = Process.CreateStream(creator, StreamName);
            long user = Process.AddUser(UserName);
            var source = Process.CreateSource(user, stream, SourceName, AccessRights.WriteOnly);
            int eventId = Process.Push(source, Message);
        }

        
    }
}
