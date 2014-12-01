using System;
using System.Collections.Generic;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvernestFront.Projection;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFrontTests
{

    [TestFixture]
    public class UserTests
    {
        private const string UserName = "userName";
        private const string StreamName = "streamName";
        private const string UserName2 = "userName2";
        private const string Message = "message";
        private const string SourceName = "sourceName";

        internal static long GetUserId_AssertSuccess(string userName)
        {
            AddUser ans = User.AddUser(userName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.UserId;
        }

        internal static User GetUser_AssertSuccess(string userName)
        {
            long userId = GetUserId_AssertSuccess(userName);
            var ans = User.GetUser(userId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.User);
            return ans.User;
        }

        internal static void SetRights_AssertSuccess(User user, long streamId, long targetUser, AccessRights rights)
        {
            SetRights ans = user.SetRights(streamId, targetUser, rights);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
        }

        int GetEventId_AssertSuccess(User user, long streamId, String message)
        {
            Push ans = user.Push(streamId, message);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.MessageId;
        }

        [SetUp]
        public void ResetTables()
        {
            Projection.Clear();
        }

        [Test]
        public void AddUser_Success()
        {
            long userId = GetUserId_AssertSuccess(UserName);
        }

       
        [Test]
        public void AddUser_UserNameTaken()
        {
            long userId = GetUserId_AssertSuccess(UserName);
            AddUser ans = User.AddUser(UserName);
            ProcessTests.ErrorAssert<UserNameTaken>(ans);
        }

        [Test]
        public void AddUser_WithPassword()
        {
            const string password = "Password";
            AddUser addUser = User.AddUser(UserName, password);
            Assert.IsTrue(addUser.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, password);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void IdentifyUser_Success()
        {
            AddUser user = User.AddUser(UserName);
            Assert.IsTrue(user.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, user.Password);
            Assert.IsTrue(ans.Success);
        }
        [Test]
        public void GetUser_Success()
        {
            long userId = GetUserId_AssertSuccess(UserName);
            var ans = User.GetUser(userId);
            Assert.IsTrue(ans.Success);
            Assert.IsNotNull(ans.User);
            Assert.IsNull(ans.Error);
        }

        [Test]
        public void GetUser_IdDoesNotExist()
        {
            var ans = User.GetUser(42);
            ProcessTests.ErrorAssert<UserIdDoesNotExist>(ans);
        }

        [Test]
        public void IdentifyUser_WrongPassword()
        {
            AddUser addUser = User.AddUser(UserName);
            Assert.IsTrue(addUser.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, "BadPassword");
            ProcessTests.ErrorAssert<WrongPassword>(ans);
        }


        [Test]
        public void SetPassword_Success()
        {
            User user = GetUser_AssertSuccess(UserName);
            const string newPassword = "NewPassword";
            SetPassword setPassword = user.SetPassword(newPassword);
            Assert.IsTrue(setPassword.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, newPassword);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void SetPassword_InvalidString()
        {
            User user = GetUser_AssertSuccess(UserName);
            const string badString = "£££££"; //non ASCII
            SetPassword setPassword = user.SetPassword(badString);
            ProcessTests.ErrorAssert<InvalidString>(setPassword);
        }

       

        [Test]
        public void SetRights_Success()
        {
            User creator = GetUser_AssertSuccess("creator");
            User reader = GetUser_AssertSuccess("reader");
            User admin = GetUser_AssertSuccess("admin");

            long streamId = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            SetRights_AssertSuccess(creator, streamId, admin.Id, AccessRights.Admin);
            SetRights_AssertSuccess(admin, streamId, reader.Id, AccessRights.ReadOnly);
            SetRights_AssertSuccess(creator, streamId, reader.Id, AccessRights.ReadWrite);
        }

        [Test]
        public void SetRights_AdminAccessDenied()
        {
            User creator = GetUser_AssertSuccess("creator");
            User reader = GetUser_AssertSuccess("reader");
            long streamId = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            SetRights_AssertSuccess(creator, streamId, reader.Id, AccessRights.ReadOnly);

            SetRights ans = reader.SetRights(streamId, reader.Id, AccessRights.ReadWrite);
            ProcessTests.ErrorAssert<AdminAccessDenied>(ans);
        }

        [Test]
        public void SetRights_CannotDestituteAdmin()
        {
            User creator = GetUser_AssertSuccess("creator");
            User evilAdmin = GetUser_AssertSuccess("evilAdmin");
            long streamId = StreamTests.GetStreamId_AssertSuccess(creator.Id, StreamName);
            SetRights_AssertSuccess(creator, streamId, evilAdmin.Id, AccessRights.Admin);

            SetRights ans = evilAdmin.SetRights(streamId, creator.Id, AccessRights.NoRights);
            ProcessTests.ErrorAssert<CannotDestituteAdmin>(ans);
        }

        [Test]
        public void Push_Success()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            int eventId = GetEventId_AssertSuccess(user, streamId, Message);
            int eventId2 = GetEventId_AssertSuccess(user, streamId, Message);
            Assert.AreEqual(eventId, 0);
            Assert.AreEqual(eventId2, 1);

        }

        [Test]
        public void Push_WriteAccessDenied()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);

            User user2 = GetUser_AssertSuccess(UserName2);
            Push ans = user2.Push(streamId, Message);
            ProcessTests.ErrorAssert<WriteAccessDenied>(ans);
        }

        [Test]
        public void Push_StreamIdDoesNotExist()
        {
            User user = GetUser_AssertSuccess(UserName);
            const long streamId = 42; //does not exist in StreamTable
            Push ans = user.Push(streamId, Message);
            ProcessTests.ErrorAssert<StreamIdDoesNotExist>(ans);
        }

        [Test]
        public void PullRandom_Success()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            int eventId = GetEventId_AssertSuccess(user, streamId, Message);
            PullRandom ans = user.PullRandom(streamId);
            Assert.IsTrue(ans.Success);
            Event pulledRandom = ans.EventPulled;
            Assert.IsNotNull(pulledRandom);
            Assert.AreEqual(eventId, pulledRandom.Id);
            Assert.AreEqual(pulledRandom.Message,Message); //will work when we connect to back-end : TODO 
        }

        [Test]
        public void Pull_Success()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            int eventId = GetEventId_AssertSuccess(user, streamId, Message);
            Pull ans = user.Pull(streamId, eventId);
            Assert.IsTrue(ans.Success);
            Event pulledById = ans.EventPulled;
            Assert.IsNotNull(pulledById);
            Assert.AreEqual(pulledById.Message, Message); //will work when we connect to back-end : TODO
        }

        [Test]
        public void PullRange_Success()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            int eventId = GetEventId_AssertSuccess(user, streamId, Message);
            int eventId2 = GetEventId_AssertSuccess(user, streamId, Message);
            PullRange ans = user.PullRange(streamId, eventId, eventId2);
            Assert.IsTrue(ans.Success);
            var pulled = ans.Events;
            Assert.AreEqual(pulled.Count, 2);
        }

        [Test]
        public void Pull_ReadAccessDenied()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            int eventId = GetEventId_AssertSuccess(user, streamId, Message);
            User user2 = GetUser_AssertSuccess(UserName2);
            SetRights_AssertSuccess(user, streamId, user2.Id, AccessRights.WriteOnly);
           
            Pull ans = user2.Pull(streamId, eventId);
            ProcessTests.ErrorAssert<ReadAccessDenied>(ans);
        }

        [Test]
        public void CreateSource_Success()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            CreateSource ans = user.CreateSource(SourceName, streamId, AccessRights.ReadWrite);
            Assert.IsTrue(ans.Success);
            String key = ans.Key;
            Assert.IsNotNull(key);
            CreateSource ans2 = user.CreateSource("source2", streamId, AccessRights.ReadWrite);
            Assert.IsTrue(ans2.Success);
            String key2 = ans2.Key;
            Assert.IsNotNull(key2);
            Assert.AreNotEqual(key, key2);
        }

        [Test]
        public void CreateSource_SourceNameTaken()
        {
            User user = GetUser_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(user.Id, StreamName);
            CreateSource ans = user.CreateSource(SourceName, streamId, AccessRights.ReadWrite);
            CreateSource ans2 = user.CreateSource(SourceName, streamId, AccessRights.Admin);
            ProcessTests.ErrorAssert<SourceNameTaken>(ans);
        }

    }
}
