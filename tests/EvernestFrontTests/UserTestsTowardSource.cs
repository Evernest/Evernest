﻿using System.Linq;
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
        private const AccessRight SomeRight = AccessRight.ReadOnly; //constant to use when the right is not decisive

        [Test]
        public void CreateSource_Success()
        {
            var userName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var user = new UserProvider().GetUser(userId).Result;
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
            var userName = Helpers.NewName;
            var userName2 = Helpers.NewName;
            var streamName = Helpers.NewName;

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
            var userName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var sourceName = Helpers.NewName;
            var source = SourceTests.CreateSource_GetKey_AssertSuccess(userId, sourceName);
            User user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.CreateSource(sourceName);
            Helpers.ErrorAssert(FrontError.SourceNameTaken, ans);
        }

        [Test]
        public void DeleteSource_Success()
        {
            var userName = Helpers.NewName;
            var sourceName = Helpers.NewName;
            var userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            var sourceKey = SourceTests.CreateSource_GetKey_AssertSuccess(userId, sourceName);
            var sourceId = new SourceProvider().GetSource(sourceKey).Result.Id;
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
            string streamName = Helpers.NewName;
            string sourceName = Helpers.NewName;
            string sourceName2 = Helpers.NewName;
            string userName = Helpers.NewName;

            long userId = UserTests.AddUser_GetId_AssertSuccess(userName);
            long streamId = EventStreamTests.CreateEventStream_GetId_AssertSuccess(userId, streamName);
            string key = SourceTests.CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName, SomeRight);
            string key2 = SourceTests.CreateSource_GetKey_AssignStream_AssertSuccess(userId, streamId, sourceName2, SomeRight);
            var sb = new SourceProvider();
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
