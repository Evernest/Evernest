using System;
using System.Collections.Generic;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

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

        


        int GetEventId_AssertSuccess(long userId, long streamId, String message)
        {
            Push ans = Process.Push(userId, streamId, message);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.MessageId;
        }

        internal static void ErrorAssert<TError>(Answer ans)
        {
            Assert.IsFalse(ans.Success);
            Assert.IsNotNull(ans.Error);
            Assert.IsInstanceOf<TError>(ans.Error);
        }



        [SetUp]
        public void Initialize()
        {
            UserTable.Clear();
            StreamTable.Clear();
        }


        [Test]
        public void Push_Success()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            int eventId2 = GetEventId_AssertSuccess(userId, streamId, Message);
            Assert.AreEqual(eventId, 0);
            Assert.AreEqual(eventId2, 1);

        }

        [Test]
        public void Push_WriteAccessDenied()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);

            long user2 = UserTests.GetUserId_AssertSuccess(UserName2);
            Push ans = Process.Push(user2, streamId, Message);
            ErrorAssert<WriteAccessDenied>(ans);
        }

        [Test]
        public void Push_StreamIdDoesNotExist()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            const long streamId = 42; //does not exist in StreamTable
            Push ans = Process.Push(userId, streamId, Message);
            ErrorAssert<StreamIdDoesNotExist>(ans);
        }

        [Test]
        public void CreateSource_Success()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            CreateSource ans = Process.CreateSource(userId, streamId, SourceName, AccessRights.ReadWrite);
            Assert.IsTrue(ans.Success);
            String key = ans.Key;
            Assert.IsNotNull(key);
            CreateSource ans2=Process.CreateSource(userId, streamId, "source2", AccessRights.ReadWrite);
            Assert.IsTrue(ans2.Success);
            String key2 = ans2.Key;
            Assert.IsNotNull(key2);
            Assert.AreNotEqual(key, key2);
        }

        [Test]
        public void CreateSource_SourceNameTaken()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            Process.CreateSource(userId, streamId, SourceName, AccessRights.ReadWrite);
            CreateSource ans = Process.CreateSource(userId, streamId, SourceName, AccessRights.Admin);
            ErrorAssert<SourceNameTaken>(ans);
        }


        [Test]
        public void RelatedStreams_Success()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            const string streamName2 = "streamName2";
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            long streamId2 = StreamTests.GetStreamId_AssertSuccess(userId, streamName2);
            RelatedStreams ans = Process.RelatedStreams(userId);
            Assert.IsTrue(ans.Success);
            var actualList = ans.Streams;
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
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long userId2 = UserTests.GetUserId_AssertSuccess(UserName2);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            UserTests.SetRights_AssertSuccess(userId, streamId, userId2, AccessRights.ReadOnly);
            RelatedUsers ans = Process.RelatedUsers(userId, streamId);
            Assert.IsTrue(ans.Success);
            var actualList = ans.Users;
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
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            PullRandom ans = Process.PullRandom(userId, streamId);
            Assert.IsTrue(ans.Success);
            Event pulledRandom = ans.EventPulled;
            Assert.IsNotNull(pulledRandom);
            Assert.AreEqual(eventId, pulledRandom.Id);
            //Assert.AreEqual(pulledRandom.Message,Message); will work when we connect to back-end
        }

        [Test]
        public void Pull_Success()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            Pull ans = Process.Pull(userId, streamId, eventId);
            Assert.IsTrue(ans.Success);
            Event pulledById = ans.EventPulled;
            Assert.IsNotNull(pulledById);
            //Assert.AreEqual(pulledById.Message, Message); will work when we connect to back-end
        }

        [Test]
        public void PullRange_Success()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            int eventId2 = GetEventId_AssertSuccess(userId, streamId, Message);
            PullRange ans = Process.PullRange(userId, streamId, eventId, eventId2);
            Assert.IsTrue(ans.Success);
            var pulled = ans.Events;
            Assert.AreEqual(pulled.Count,2);
        }

        [Test]
        public void Pull_ReadAccessDenied()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            int eventId = GetEventId_AssertSuccess(userId, streamId, Message);
            long userId2 = UserTests.GetUserId_AssertSuccess(UserName2);
            UserTests.SetRights_AssertSuccess(userId,streamId,userId2,AccessRights.WriteOnly);
            
            Pull ans = Process.Pull(userId2, streamId, eventId);
            ErrorAssert<ReadAccessDenied>(ans);
        }

        

        [Test]
        public void SetPassword_Success()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            const string newPassword = "NewPassword";
            SetPassword setPassword = Process.SetPassword(userId, newPassword);
            Assert.IsTrue(setPassword.Success);
            IdentifyUser ans = Process.IdentifyUser(UserName, newPassword);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void SetPassword_InvalidString()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            const string badString = "£££££"; //non ASCII
            SetPassword setPassword = Process.SetPassword(userId, badString);
            ErrorAssert<InvalidString>(setPassword);
        }

        [Test]
        public void AddUser_WithPassword()
        {
            const string password = "Password";
            AddUser addUser = Process.AddUser(UserName, password);
            Assert.IsTrue(addUser.Success);
            IdentifyUser ans = Process.IdentifyUser(UserName, password);
            Assert.IsTrue(ans.Success);
        }
    }      
}
