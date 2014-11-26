using System;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFrontTests
{   
    [TestFixture]
    class StreamTests
    {
        private const string UserName = "userName";
        private const string StreamName = "streamName";
        private const string UserName2 = "userName2";
        private const string Message = "message";
        private const string SourceName = "sourceName";


        internal static long GetStreamId_AssertSuccess(long userId, string streamName)
        {
            CreateStream ans = Stream.CreateStream(userId, streamName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.StreamId;
        }


        [Test]
        public void CreateStream_Success()
        {
            long userId = UserTests.GetUserId_AssertSuccess(UserName);
            long streamId = StreamTests.GetStreamId_AssertSuccess(userId, StreamName);
        }

        [Test]
        public void CreateStream_UserIdDoesNotExist()
        {
            const long bogusUserId = 42; //does not exist in UserTable
            CreateStream ans = Stream.CreateStream(bogusUserId, StreamName);
            ProcessTests.ErrorAssert<UserIdDoesNotExist>(ans);
        }

        [Test]
        public void CreateStream_StreamNameTaken()
        {
            long user = UserTests.GetUserId_AssertSuccess(UserName);
            long user2 = UserTests.GetUserId_AssertSuccess(UserName2);

            long streamId = StreamTests.GetStreamId_AssertSuccess(user, StreamName);
            CreateStream ans = Stream.CreateStream(user2, StreamName);
            ProcessTests.ErrorAssert<StreamNameTaken>(ans);
        }


    }
}
