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
            User user = UserTests.GetUser_AssertSuccess(UserName); //TODO : remove the rest
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long userId2 = UserTests.GetUserId_AssertSuccess(UserName2);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
            UserTests.SetRights_AssertSuccess(user, streamId, userId2, AccessRights.ReadOnly);
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

       

        

    }      
}
