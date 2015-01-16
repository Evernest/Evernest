using EvernestFront;
using EvernestFront.Projection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;

namespace EvernestFrontTests
{

    [TestFixture]
    public class UserTests
    {
        private const string UserName = "userName";
        private const string UserName2 = "userName2";

        internal static long AddUser_GetId_AssertSuccess(string userName)
        {
            AddUser ans = User.AddUser(userName);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            return ans.UserId;
        }

        internal static User GetUser_AssertSuccess(long userId)
        {
            var ans = User.GetUser(userId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.User);
            return ans.User;
        }

        

        [SetUp]
        public void ResetTables()
        {
            //TODO : clear tables ?
            Setup.ClearAsc();
        }

        [Test]
        public void AddUser_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
        }

       
        [Test]
        public void AddUser_UserNameTaken()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            AddUser ans = User.AddUser(UserName);
            AssertAuxiliaries.ErrorAssert(FrontError.UserNameTaken,ans);
        }

        [Test]
        public void AddUser_WithPassword()
        {
            const string password = "Password";
            AddUser addUser = User.AddUser(UserName, password);
            Assert.IsTrue(addUser.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, password);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void IdentifyUser_Success()
        {
            AddUser user = User.AddUser(UserName);
            Assert.IsTrue(user.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, user.Password);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void IdentifyUser_UserNameDoesNotExist()
        {
            const string inexistentUserName = "InexistentUserName";
            var ans = User.IdentifyUser(inexistentUserName, "password");
            AssertAuxiliaries.ErrorAssert(FrontError.UserNameDoesNotExist,ans);
        }

        [Test]
        public void IdentifyUser_WrongPassword()
        {
            AddUser addUser = User.AddUser(UserName);
            Assert.IsTrue(addUser.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, "WrongPassword");
            AssertAuxiliaries.ErrorAssert(FrontError.WrongPassword,ans);
        }

        [Test]
        public void GetUser_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            var ans = User.GetUser(userId);
            Assert.IsTrue(ans.Success);
            Assert.IsNotNull(ans.User);
            Assert.IsNull(ans.Error);
        }

        [Test]
        public void GetUser_IdDoesNotExist()
        {
            var ans = User.GetUser(42);
            AssertAuxiliaries.ErrorAssert(FrontError.UserIdDoesNotExist,ans);
        }

        


        [Test]
        public void SetPassword_Success()
        {
            const string initialPassword = "InitialPassword";
            var userAdded = User.AddUser(UserName, initialPassword);
            var userId = userAdded.UserId;
            const string newPassword = "NewPassword";
            User user = GetUser_AssertSuccess(userId);
            SetPassword setPassword = user.SetPassword(initialPassword, newPassword);
            Assert.IsTrue(setPassword.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, newPassword);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void SetPassword_WrongPassword()
        {
            const string initialPassword = "InitialPassword";
            var userAdded = User.AddUser(UserName, initialPassword);
            var userId = userAdded.UserId;
            const string newPassword = "NewPassword";
            User user = GetUser_AssertSuccess(userId);
            SetPassword setPassword = user.SetPassword("WrongPassword", newPassword);
            AssertAuxiliaries.ErrorAssert(FrontError.WrongPassword,setPassword);
        }

        [Test]
        public void SetPassword_InvalidString()
        {
            const string initialPassword = "InitialPassword";
            var userAdded = User.AddUser(UserName, initialPassword);
            var userId = userAdded.UserId;
            const string badString = "£££££"; //non ASCII
            User user = GetUser_AssertSuccess(userId);
            SetPassword setPassword = user.SetPassword(initialPassword, badString);
            AssertAuxiliaries.ErrorAssert(FrontError.InvalidString,setPassword);
        }

        

        
    }
}
