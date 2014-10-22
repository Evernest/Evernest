using System;
using EvernestFront;
using EvernestFront.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;


namespace EvernestFrontTests
{
    [TestFixture]
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
        //    RightsTable.AddUser(user);
        //    RightsTable.AddStream(user, stream);
        //}

        [TearDown]
        public void Cleanup()
        {
            RightsTable.ResetTable();
        }



        [Test]
        public void AddStream_StreamNameNotTaken_SetsCreatorRights()
            //nom ?
        {
            RightsTable.AddUser(user);
            RightsTable.AddStream(user, stream);
            var expected = Users.CreatorRights;
            var actual = RightsTable.GetRights(user, stream);
            Assert.AreEqual(expected, actual);
        }

        [Test] 
        [ExpectedException(typeof(StreamNameTakenException))]
        public void AddStream_StreamNameTaken_Throws()
        {
            RightsTable.AddUser(user);
            RightsTable.AddStream(user, stream);
            RightsTable.AddUser(user1);
            RightsTable.AddStream(user1,stream);
        }

        [Test]
        [ExpectedException(typeof(StreamNameDoesNotExistException))]
        public void SetRights_StreamNameDoesNotExist_Throws()
        {
            RightsTable.AddUser(user);
            RightsTable.SetRights(user, stream, AccessRights.ReadOnly);
        }


        // ce n'est plus dans cette classe

        //[Test]
        //[ExpectedException(typeof(AccessDeniedException))]
        //public void CheckCanRead_NoRights_AccessDeniedException()
        //{
        //    RightsTable.AddStream(user, stream);
        //    CheckRights.CheckCanRead(user1, stream);
        //}


        //[Test]
        //public void CheckCanRead_Read_Returns()
        //{
        //    RightsTable.AddStream(user, stream);
        //    RightsTable.SetRights(user1, stream, AccessRights.ReadOnly);
        //    CheckRights.CheckCanRead(user1, stream);
        //}


        //[Test]
        //public void CheckCanRead_ReadWrite_Returns()
        //{
        //    RightsTable.AddStream(user, stream);
        //    RightsTable.SetRights(user1, stream, AccessRights.ReadWrite);
        //    CheckRights.CheckCanRead(user1, stream);
        //}


    }
}
