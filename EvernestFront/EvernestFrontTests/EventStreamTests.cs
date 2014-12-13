using System;
using System.Collections.Generic;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFrontTests
{   
    [TestFixture]
    class EventStreamTests
    {
        private const string UserName = "userName";
        private const string StreamName = "streamName";
        private const string UserName2 = "userName2";
        private const string Message = "message";
        private const string SourceName = "sourceName";


        

        [SetUp]
        public void Initialize()
        {
            Projection.Clear();
        }


        

        [Test]
        public void GetEventStream()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            long streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);

            var ans = EventStream.GetStream(streamId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            var stream = ans.EventStream;
            Assert.AreEqual(streamId, stream.Id);
            Assert.AreEqual(StreamName, stream.Name);
            var relatedUsers = new List<KeyValuePair<long, AccessRights>>();
            relatedUsers.Add(new KeyValuePair<long, AccessRights>(userId,AccessRights.Admin));
            //const creatorRight when it is implemented
            Assert.AreEqual(relatedUsers, stream.RelatedUsers);
            Assert.AreEqual(0, stream.Count);
            //LastEventId?
        }
    }
}
