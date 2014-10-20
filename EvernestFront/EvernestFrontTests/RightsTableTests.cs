﻿using System;
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

        



        //[TestInitialize]
        //public void Initialize()
        //{
        //    RightsTable.AddStream(user, stream);
        //}


        [TestCleanup]
        public void Cleanup()
        {
            RightsTable.ResetTable();
        }



        [TestMethod]
        public void AddStream_StreamNameNotTaken_SetsCreatorRights()
            //nom ?
        {
            RightsTable.AddStream(user, stream);
            var expected = RightsTable.CreatorRights;
            var actual = RightsTable.GetRights(user, stream);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod] 
        [ExpectedException(typeof(StreamNameTakenException))]
        public void AddStream_StreamNameTaken_Throws()
        {
            RightsTable.AddStream(user, stream);
            RightsTable.AddStream(user1,stream);
        }

        [TestMethod]
        [ExpectedException(typeof(StreamNameDoesNotExistException))]
        public void SetRights_StreamNameDoesNotExist_Throws()
        {
            RightsTable.SetRights(user, stream, AccessRights.Read);
        }

        [TestMethod]
        [ExpectedException(typeof(AccessDeniedException))]
        public void CheckCanRead_NoRights_AccessDeniedException()
        {
            RightsTable.AddStream(user, stream);
            RightsTable.CheckCanRead(user1,stream);
        }

        [TestMethod]
        public void CheckCanRead_Read_Returns()
        {
            RightsTable.AddStream(user, stream);
            RightsTable.SetRights(user1, stream, AccessRights.Read);
            RightsTable.CheckCanRead(user1, stream);
        }

        [TestMethod]
        public void CheckCanRead_ReadWrite_Returns()
        {
            RightsTable.AddStream(user, stream);
            RightsTable.SetRights(user1, stream, AccessRights.ReadWrite);
            RightsTable.CheckCanRead(user1, stream);
        }


    }
}
