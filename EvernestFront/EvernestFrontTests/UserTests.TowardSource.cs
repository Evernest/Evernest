using System;
using EvernestFront;
using EvernestFront.Answers;
using EvernestFront.Errors;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace EvernestFrontTests
{

    [TestFixture]
    public partial class UserTests
    {

        private const string SourceName = "sourceName";

        [Test]
        public void CreateSource_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            User user = GetUser_AssertSuccess(userId);
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
            long userId = AddUser_GetId_AssertSuccess(UserName);
            long streamId = CreateEventStream_GetId_AssertSuccess(userId, StreamName);
            User user = GetUser_AssertSuccess(userId);
            CreateSource ans = user.CreateSource(SourceName, streamId, AccessRights.ReadWrite);
            user = GetUser_AssertSuccess(userId);
            CreateSource ans2 = user.CreateSource(SourceName, streamId, AccessRights.Admin);
            AssertAuxiliaries.ErrorAssert<SourceNameTaken>(ans2);
        }
    }
}
