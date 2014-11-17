using System.Collections.Generic;
using EvernestFront;
using EvernestFront.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using KeyType = System.String;

namespace EvernestFrontTests
{

    [TestFixture]
    public class ProcessTests
    {
        private const string UserName = "userName";
        private const string StreamName = "streamName";
        private const string UserName2 = "userName2";
        private const string Message = "message";
        private const string SourceName = "sourceName";

        [SetUp]
        public void Initialize()
        {
            UserTable.Clear();
            StreamTable.Clear();
        }

        [Test]
        public void AddUser_Success()
        {
            long userId = Process.AddUser(UserName);

            //check that the user has been added to UserTable using internal methods

            User user = UserTable.GetUser(userId);
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual(UserName, user.Name);

            //check that IDs are distinct

            long userId2 = Process.AddUser(UserName2);
            Assert.AreNotEqual(userId, userId2);
        }

        [Test]
        [ExpectedException(typeof (UserNameTakenException))]
        public void AddUser_UserNameTaken()
        {
            long userId = Process.AddUser(UserName);
            long userId2 = Process.AddUser(UserName);
        }

        [Test]
        public void CreateStream_Success()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);

            //check that the user has creator rights using internal methods...

            User user = UserTable.GetUser(userId);
            Stream stream = StreamTable.GetStream(streamId);
            AccessRights rights = UserRight.GetRight(user, stream);
            Assert.AreEqual(rights, UserRight.CreatorRights);
        }

        [Test]
        [ExpectedException(typeof (UserIdDoesNotExistException))]
        public void CreateStream_UserIdDoesNotExist()
        {
            const long bogusUserId = 42;
            long streamId = Process.CreateStream(bogusUserId, StreamName);
        }

        [Test]
        [ExpectedException(typeof (StreamNameTakenException))]
        public void CreateStream_StreamNameTaken()
        {
            long user = Process.AddUser(UserName);
            long user2 = Process.AddUser(UserName2);

            long streamId = Process.CreateStream(user, StreamName);
            long stream2 = Process.CreateStream(user2, StreamName);
        }

        [Test]
        public void SetRights_Success()
        {
            long creator = Process.AddUser("creator");
            long reader = Process.AddUser("reader");
            //long writer = Process.AddUser("writer");
            //long readwriter = Process.AddUser("readwriter");
            long admin = Process.AddUser("admin");
            //long outsider = Process.AddUser("outsider");

            long streamId = Process.CreateStream(creator, StreamName);
            Process.SetRights(creator, streamId, admin, AccessRights.Admin);
            Process.SetRights(admin, streamId, reader, AccessRights.ReadOnly);
            Process.SetRights(creator, streamId, reader, AccessRights.ReadWrite);
        }

        [Test]
        [ExpectedException(typeof (AdminAccessDeniedException))]
        public void SetRights_AdminAccessDenied()
        {
            long creator = Process.AddUser("creator");
            long reader = Process.AddUser("reader");
            long streamId = Process.CreateStream(creator, StreamName);
            Process.SetRights(creator, streamId, reader, AccessRights.ReadOnly);

            Process.SetRights(reader, streamId, reader, AccessRights.ReadWrite);
        }

        [Test]
        [ExpectedException(typeof (CannotDestituteAdminException))]
        public void SetRights_CannotDestituteAdmin()
        {
            long creator = Process.AddUser("creator");
            long evilAdmin = Process.AddUser("evilAdmin");
            long streamId = Process.CreateStream(creator, StreamName);
            Process.SetRights(creator, streamId, evilAdmin, AccessRights.Admin);

            Process.SetRights(evilAdmin, streamId, creator, AccessRights.NoRights);
        }

        [Test]
        public void Push_Success()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);
            int eventId = Process.Push(userId, streamId, Message);
            int eventId2 = Process.Push(userId, streamId, Message);
            Assert.AreEqual(eventId, 0);
            Assert.AreEqual(eventId2, 1);

        }

        [Test]
        [ExpectedException(typeof (WriteAccessDeniedException))]
        public void Push_WriteAccessDenied()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);

            long user2 = Process.AddUser(UserName2);
            int eventId = Process.Push(user2, streamId, Message);
        }

        [Test]
        [ExpectedException(typeof (StreamIdDoesNotExistException))]
        public void Push_StreamIdDoesNotExist()
        {
            long userId = Process.AddUser(UserName);
            const long streamId = 42; //does not exist in StreamTable
            Process.Push(userId, streamId, Message);
        }

        [Test]
        public void CreateSource_Success()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);
            KeyType key = Process.CreateSource(userId, streamId, SourceName, AccessRights.ReadWrite);

            KeyType key2 = Process.CreateSource(userId, streamId, "source2", AccessRights.ReadWrite);
            Assert.AreNotEqual(key, key2);
        }

        [Test]
        [ExpectedException(typeof (SourceNameTakenException))]
        public void CreateSource_SourceNameTaken()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);
            Process.CreateSource(userId, streamId, SourceName, AccessRights.ReadWrite);
            Process.CreateSource(userId, streamId, SourceName, AccessRights.Admin);
        }


        [Test]
        public void RelatedStreams_Success()
        {
            long userId = Process.AddUser(UserName);
            const string streamName2 = "streamName2";
            long streamId = Process.CreateStream(userId, StreamName);
            long streamId2 = Process.CreateStream(userId, streamName2);
            var actualList = Process.RelatedStreams(userId);
            Assert.IsNotNull(actualList);
            var expected1 = new KeyValuePair<long, AccessRights>(streamId, UserRight.CreatorRights);
            var expected2 = new KeyValuePair<long, AccessRights>(streamId2, UserRight.CreatorRights);
            Assert.Contains(expected1, actualList);
            Assert.Contains(expected2, actualList);
            Assert.AreEqual(actualList.Count, 2);

        }

        [Test]
        public void RelatedUsers_Success()
        {
            long userId = Process.AddUser(UserName);
            long userId2 = Process.AddUser(UserName2);
            long streamId = Process.CreateStream(userId, StreamName);
            Process.SetRights(userId, streamId, userId2, AccessRights.ReadOnly);
            var actualList = Process.RelatedUsers(userId, streamId);
            var expected1 = new KeyValuePair<long, AccessRights>(userId, UserRight.CreatorRights);
            var expected2 = new KeyValuePair<long, AccessRights>(userId2, AccessRights.ReadOnly);
            Assert.IsNotNull(actualList);
            Assert.Contains(expected1, actualList);
            Assert.Contains(expected2, actualList);
            Assert.AreEqual(actualList.Count, 2);
        }

        [Test]
        public void PullRandom_Success()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);
            int eventId = Process.Push(userId, streamId, Message);
            Event pulledRandom = Process.PullRandom(userId, streamId);
            Assert.IsNotNull(pulledRandom);
            Assert.AreEqual(eventId, pulledRandom.Id);
        }

        [Test]
        public void Pull_Success()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);
            int eventId = Process.Push(userId, streamId, Message);
            Event pulledById = Process.Pull(userId, streamId, eventId);
            Assert.IsNotNull(pulledById);
        }

        [Test]
        public void PullRange_Success()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);
            int eventId = Process.Push(userId, streamId, Message);
            int eventId2 = Process.Push(userId, streamId, Message);
            var pulled = Process.PullRange(userId, streamId, eventId, eventId2);
            Assert.AreEqual(pulled.Count,2);
        }

        [Test]
        [ExpectedException(typeof (ReadAccessDeniedException))]
        public void Pull_ReadAccessDenied()
        {
            long userId = Process.AddUser(UserName);
            long streamId = Process.CreateStream(userId, StreamName);
            int eventId = Process.Push(userId, streamId, Message);
            long userId2 = Process.AddUser(UserName2);
            Process.SetRights(userId,streamId,userId2,AccessRights.WriteOnly);
            Process.Pull(userId2, streamId, eventId);
        }
}      
}
