using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using EvernestFront;
using EvernestFront.Contract;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;


namespace EvernestFrontTests
{
    [TestFixture]
    class SerializingTests
    {
        private const string UserName = "userName";
        private const string Message = "message";

        [SetUp]
        public void ResetTables()
        {
            ProjectionOld.Clear();
        }


        [Test]
        public static void ReadContract_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var date = DateTime.UtcNow;
            var contract = new EventContract(user, date, Message);
            var serializedContract = Serializing.WriteContract<EventContract>(contract);
            var deserializedContract = Serializing.ReadContract<EventContract>(serializedContract);
            Assert.AreEqual(contract.AuthorId, deserializedContract.AuthorId);
            Assert.AreEqual(contract.AuthorName, deserializedContract.AuthorName);
            Assert.AreEqual(contract.Date, deserializedContract.Date);
            Assert.AreEqual(contract.Message, deserializedContract.Message);
        }
    }

}
