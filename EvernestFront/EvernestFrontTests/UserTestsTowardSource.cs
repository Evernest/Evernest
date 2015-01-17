using System.Collections.Generic;
using EvernestFront;
using EvernestFront.Responses;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace EvernestFrontTests
{

    [TestFixture]
    public class UserTestsTowardSources
    {
        private const string UserName = "userName";
        private const string UserName2 = "userName2";
        private const string StreamName = "streamName";
        private const string SourceName = "sourceName";
        private const string SourceName2 = "sourceName2";
        private const AccessRight SomeRight = AccessRight.ReadOnly; //constant to use when the right is not decisive

        [SetUp]
        public void ResetTables()
        {
            //TODO : clear tables ?
            Setup.ClearAsc();
        }

        [Test]
        public void CreateSource_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            User user = UserTests.GetUser_AssertSuccess(userId);
            CreateSource ans = user.CreateSource(SourceName, streamId, AccessRight.ReadWrite);
            Assert.IsTrue(ans.Success);
            string key = ans.Key;
            Assert.IsNotNull(key);
            CreateSource ans2 = user.CreateSource("source2", streamId, AccessRight.ReadWrite);
            Assert.IsTrue(ans2.Success);
            string key2 = ans2.Key;
            Assert.IsNotNull(key2);
            Assert.AreNotEqual(key, key2);
        }

        [Test]
        public void CreateSource_SourceNameTaken()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            User user = UserTests.GetUser_AssertSuccess(userId);
            CreateSource ans = user.CreateSource(SourceName, streamId, SomeRight);
            user = UserTests.GetUser_AssertSuccess(userId);
            CreateSource ans2 = user.CreateSource(SourceName, streamId, SomeRight);
            AssertAuxiliaries.ErrorAssert(FrontError.SourceNameTaken,ans2);
        }

        [Test]
        public void Sources_Property()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            string key = SourceTests.CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName, SomeRight);
            string key2 = SourceTests.CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName2, SomeRight);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var sources = user.Sources;
            Assert.AreEqual(2, sources.Count);
            Assert.IsTrue(sources.Contains(new KeyValuePair<string, string>(SourceName, key)));
            Assert.IsTrue(sources.Contains(new KeyValuePair<string, string>(SourceName2, key2)));
        }

        [Test]
        public void DeleteSource_Success()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            var sourceKey = SourceTests.CreateSource_GetKey_AssertSuccess(userId, streamId, SourceName, SomeRight);
            User user = UserTests.GetUser_AssertSuccess(userId);
            DeleteSource ds = user.DeleteSource(SourceName);
            Assert.IsTrue(ds.Success);
            user = UserTests.GetUser_AssertSuccess(userId);
            Assert.IsFalse(user.Sources.Exists(pair => pair.Key==SourceName));
        }
    }
}
