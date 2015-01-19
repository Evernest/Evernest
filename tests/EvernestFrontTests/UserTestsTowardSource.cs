using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EvernestFront;
using EvernestFront.Contract;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

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
            var userName = AssertAuxiliaries.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var user = new UsersProvider().GetUser(userId).Result;
            Assert.IsNotNull(user);
            var create1 = user.CreateSource("source1");
            var create2 = user.CreateSource("source2");
            Assert.IsTrue(create1.Success);
            Assert.IsTrue(create2.Success);
            Assert.AreNotEqual(create1.Result.Item1, create2.Result.Item1);
        }

        [Test]
        public void CreateSource_SameNameDistinctUsers_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            var userName2 = AssertAuxiliaries.NewName;
            var streamName = AssertAuxiliaries.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(userId, streamName);
            var sourceKey = SourceTests.CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, "SourceName", SomeRight);
            var user2Id = UserTests.AddUser_GetId_AssertSuccess(userName2);
            var source2Key = SourceTests.CreateSource_GetKey_AssignStream_AssertSuccess(user2Id, streamId, "SourceName", SomeRight);
            Assert.AreNotEqual(sourceKey, source2Key);
        }

        [Test]
        public void CreateSource_SourceNameTaken()
        {
            var userName = AssertAuxiliaries.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var sourceName = AssertAuxiliaries.NewName;
            var source = SourceTests.CreateSource_GetKey_AssertSuccess(userId, sourceName);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.CreateSource(sourceName);
            AssertAuxiliaries.ErrorAssert(FrontError.SourceNameTaken, ans);
        }

        [Test]
        public void DeleteSource_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            var sourceName = AssertAuxiliaries.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var sourceKey = SourceTests.CreateSource_GetKey_AssertSuccess(userId, sourceName);
            var sourceId = new SourcesProvider().GetSource(sourceKey).Result.Id;
            var user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.DeleteSource(sourceId);
            Assert.IsTrue(ans.Success);
            Thread.Sleep(100);
            user = UserTests.GetUser_AssertSuccess(userId);
            Assert.IsFalse(user.Sources.Contains(sourceId));

        }

        [Test]
        public void Sources_Property()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            string key = SourceTests.CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, SourceName, SomeRight);
            string key2 = SourceTests.CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, SourceName2, SomeRight);
            var sb = new SourcesProvider();
            var source1 = sb.GetSource(key).Result;
            var source2 = sb.GetSource(key2).Result;
            User user = UserTests.GetUser_AssertSuccess(userId);
            var sources = user.Sources.ToList();
            Assert.AreEqual(2, sources.Count());
            Assert.IsTrue(sources.Contains(source1.Id));
            Assert.IsTrue(sources.Contains(source2.Id));
        }

    }
}
