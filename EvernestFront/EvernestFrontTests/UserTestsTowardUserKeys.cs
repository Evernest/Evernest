using EvernestFront;
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
            CreateUserKey ans = user.CreateUserKey(keyName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.Key;
        }
        internal static string CreateUserKey_ReturnKey_AssertSuccess(User user)
        {
            CreateUserKey ans = user.CreateUserKey();
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.Key;
        }

        [SetUp]
        public void ResetTables()
        {
            //TODO : reset tables ?
            Setup.ClearAsc();
        }

        [Test]
        public void CreateUserKey_DefaultName_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user);
        }

        [Test]
        public void CreateUserKey_GivenName_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, KeyName);
        }


        [Test]
        [Ignore]
        public void CreateUserKey_SuccessiveDefaultNames_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user);
            user = UserTests.GetUser_AssertSuccess(userId);
            var key2 = CreateUserKey_ReturnKey_AssertSuccess(user);
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
            var ans = User.GetUser(key);
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
            var ans = User.GetUser(inexistantKey);
            AssertAuxiliaries.ErrorAssert(FrontError.UserKeyDoesNotExist,ans);
        }

        [Test]
        public void IdentifyUser_Success()
        {
            var userId = UserTests.AddUser_GetId_AssertSuccess(UserName);
            var user = UserTests.GetUser_AssertSuccess(userId);
            var key = CreateUserKey_ReturnKey_AssertSuccess(user, KeyName);
            var ans = User.IdentifyUser(key);
            Assert.IsTrue(ans.Success);
            var user2 = ans.User;
            Assert.AreEqual(userId, user2.Id);
            Assert.AreEqual(user.Name, user2.Name);
            //other asserts?
        }
    }
}
