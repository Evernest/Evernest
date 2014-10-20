using System;
using EvernestFront;
using EvernestFront.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NUnit.Framework;
using Assert = NUnit.Framework.Assert;


namespace EvernestFrontTests
{
    [TestClass]
    public class RightsTableTests
    {

        // DRAFT

        private string user = "some user name";
        private string user1 = "some other user name";
        private string stream = "some stream name";
        private string stream1 = "some other stream name";

        

        //[SetUp]
        //public void Initialize()
        //{
        //    RightsTable.AddStream(user, stream);
        //}

        //[NUnit.Framework.TearDown]
        //public void Reset()
        //{
        //    RightsTable.ResetTable();
        //}


        [TestMethod]
        public void AddStream_StreamNameNotTaken_SetsCreatorRights()
            //nom ?
        {
            RightsTable.ResetTable();
            RightsTable.AddStream(user, stream);
            var expected = RightsTable.CreatorRights;
            var actual = RightsTable.GetRights(user, stream);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod] 
        [ExpectedException(typeof(StreamNameTakenException))]
        public void AddStream_StreamNameTaken_Throws()
        {
            RightsTable.ResetTable();
            RightsTable.AddStream(user, stream);
            RightsTable.AddStream(user1,stream);
        }

        [TestMethod]
        [ExpectedException(typeof(StreamNameDoesNotExistException))]
        public void SetRights_StreamNameDoesNotExist_Throws()
        {
            RightsTable.ResetTable();
            RightsTable.SetRights(user, stream, AccessRights.Read);
        }

        [TestMethod]
        [ExpectedException(typeof(AccessDeniedException))]
        public void CheckCanRead_AccessRightsDotNoRights_AccessDeniedException()
        {
            RightsTable.ResetTable();
            RightsTable.AddStream(user, stream);
            RightsTable.CheckCanRead(user1,stream);
        }

        [TestMethod]
        public void CheckCanRead_AccessRightsDotRead_Returns()
        {
            RightsTable.ResetTable();
            RightsTable.AddStream(user, stream);
            RightsTable.SetRights(user1, stream, AccessRights.Read);
            RightsTable.CheckCanRead(user1, stream);
        }

        [TestMethod]
        public void CheckCanRead_AccessRightsDotReadWrite_Returns()
        {
            RightsTable.ResetTable();
            RightsTable.AddStream(user, stream);
            RightsTable.SetRights(user1, stream, AccessRights.ReadWrite);
            RightsTable.CheckCanRead(user1, stream);
        }


    }
}
