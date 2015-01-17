using System.Collections.Generic;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

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
            //TODO : clear tables ?
            Setup.ClearAsc();
        }


        

        [Test]
        public void GetEventStream()
        {
            long userId = UserTests.AddUser_GetId_AssertSuccess(AssertAuxiliaries.NewName);
            string StreamName = AssertAuxiliaries.NewName;
            long streamId = UserTestsTowardEventStream.CreateEventStream_GetId_AssertSuccess(userId, StreamName);

            var ans = EventStream.GetStream(streamId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            var stream = ans.EventStream;
            Assert.AreEqual(streamId, stream.Id);
            Assert.AreEqual(StreamName, stream.Name);
            var relatedUsers = new List<KeyValuePair<long, AccessRight>>();
            relatedUsers.Add(new KeyValuePair<long, AccessRight>(userId,AccessRight.Admin));
            //const creatorRight when it is implemented
            Assert.AreEqual(relatedUsers, stream.RelatedUsers);
            Assert.AreEqual(0, stream.Count);
            //LastEventId?
        }
    }
}
