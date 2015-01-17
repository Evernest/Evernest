using System;
using EvernestFront;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;


namespace EvernestFrontTests
{
    [TestFixture]
    class SerializerTests
    {
        private const string Message = "message";

        [SetUp]
        public void ResetTables()
        {
            //TODO : clear tables ?
            Setup.ClearAsc();
        }


        [Test]
        public static void ReadContract_Success()
        {
            var serializer = new Serializer();
            var userId = UserTests.AddUser_GetId_AssertSuccess(AssertAuxiliaries.NewName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var date = DateTime.UtcNow;
            var contract = new EventContract(user, date, Message);
            var serializedContract = serializer.WriteContract<EventContract>(contract);
            var deserializedContract = serializer.ReadContract<EventContract>(serializedContract);
            Assert.AreEqual(contract.AuthorId, deserializedContract.AuthorId);
            Assert.AreEqual(contract.AuthorName, deserializedContract.AuthorName);
            Assert.AreEqual(contract.Date, deserializedContract.Date);
            Assert.AreEqual(contract.Message, deserializedContract.Message);
        }
    }

}
