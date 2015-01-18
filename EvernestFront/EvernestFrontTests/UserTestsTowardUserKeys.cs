using System.Linq;
using EvernestFront;
using EvernestFront.Responses;
using NUnit.Framework;

namespace EvernestFrontTests
{
    [TestFixture]
    class UserTestsTowardUserKeys
    {
        private const string UserName = "userName";
        private const string UserName2 = "userName2";
        private const string KeyName = "keyName";

        internal static string CreateUserKey_ReturnKey_AssertSuccess(User user, string keyName)
        {
            SystemCommandResponse ans = user.CreateUserKey(keyName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            System.Threading.Thread.Sleep(100);
            string key = null;
            Assert.IsTrue(user.TryGetUserKey(keyName, out key));
            return key;
        }

        [SetUp]
        public void ResetTables()
        {
            //TODO : reset tables ?
            Setup.ClearAsc();
        }

        [Test]
        public void CreateUserKey_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, KeyName);
        }


        [Test]
        public void CreateUserKey_UserKeyNameTaken()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, KeyName);
            user = UserTests.GetUser_AssertSuccess(userId);
            var ans = user.CreateUserKey(KeyName);
            AssertAuxiliaries.ErrorAssert(FrontError.UserKeyNameTaken,ans);
        }

        [Test]
        public void GetUser_FromUserKey_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, KeyName);
            var usb = new UsersBuilder();
            var ans = usb.GetUser(key);
            Assert.IsTrue(ans.Success);
            var actualUser = ans.User;
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
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, KeyName);
            var usb = new UsersBuilder();
            var ans = usb.IdentifyUser(key);
            Assert.IsTrue(ans.Success);
            var user2 = ans.User;
            Assert.AreEqual(userId, user2.Id);
            Assert.AreEqual(user.Name, user2.Name);
            //other asserts?
        }
    }
}
