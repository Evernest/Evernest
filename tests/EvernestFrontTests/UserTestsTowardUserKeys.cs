using System;
using EvernestFront;
using EvernestFront.Contract;
using NUnit.Framework;

namespace EvernestFrontTests
{
    [TestFixture]
    class UserTestsTowardUserKeys
    {
        private string _userName;
        private string _userName2;
        private string _keyName;

        internal static string CreateUserKey_ReturnKey_AssertSuccess(User user, string keyName)
        {
            Response<Tuple<string, Guid>> ans = user.CreateUserKey(keyName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            System.Threading.Thread.Sleep(100);
            return ans.Result.Item1;
        }

        [SetUp]
        public void ResetTables()
        {
            _userName = AssertAuxiliaries.NewName;
            _userName2 = AssertAuxiliaries.NewName;
            _keyName = AssertAuxiliaries.NewName;
            Setup.ClearAsc();
        }

        [Test]
        public void CreateUserKey_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(_userName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, _keyName);
        }


        [Test]
        public void CreateUserKey_UserKeyNameTaken()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(_userName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, _keyName);
            user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.CreateUserKey(_keyName);
            AssertAuxiliaries.ErrorAssert(FrontError.UserKeyNameTaken,ans);
        }

        [Test]
        public void GetUser_FromUserKey_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(_userName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, _keyName);
            var usb = new UsersBuilder();
            var ans = usb.GetUser(key);
            Assert.IsTrue(ans.Success);
            var actualUser = ans.Result;
            Assert.AreEqual(userId, actualUser.Id);
            Assert.AreEqual(user.Name, actualUser.Name);
            //other asserts?
        }

        [Test]
        public void GetUser_FromUserKey_UserKeyDoesNotExist()
        {
            const string inexistantKey = "InexistantKey";
            var usb = new UsersBuilder();
            var ans = usb.GetUser(inexistantKey);
            AssertAuxiliaries.ErrorAssert(FrontError.UserKeyDoesNotExist,ans);
        }

        [Test]
        public void IdentifyUser_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(_userName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, _keyName);
            var usb = new UsersBuilder();
            var ans = usb.IdentifyUser(key);
            Assert.IsTrue(ans.Success);
            var user2 = ans.Result;
            Assert.AreEqual(userId, user2.Id);
            Assert.AreEqual(user.Name, user2.Name);
            //other asserts?
        }
    }
}
