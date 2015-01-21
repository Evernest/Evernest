using System;
using EvernestFront;
using EvernestFront.Contract;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace EvernestFrontTests
{

    [TestFixture]
    public class UserTests
    {
        private const string Password = "password";

        internal static long AddUser_GetId_AssertSuccess(string userName)
        {
            var usp = new UserProvider();
            Response<Guid> add = usp.AddUser(userName, Password);
            Assert.IsTrue(add.Success);
            Assert.IsNull(add.Error);
            var viewer = new SystemCommandResultViewer();
            Response<Guid> response;
            while (!viewer.TryGetResult(add.Result, out response))
            {
                System.Threading.Thread.Sleep(50);
            }
            Assert.IsTrue(response.Success);
            Assert.IsNull(response.Error);
            Response<User> ans = usp.IdentifyUser(userName, Password);
            return ans.Result.Id;
        }

        internal static User GetUser_AssertSuccess(long userId)
        {
            var usp = new UserProvider();
            var ans = usp.GetUser(userId);
            Assert.IsTrue(ans.Success);
            Assert.IsNull(ans.Error);
            Assert.IsNotNull(ans.Result);
            return ans.Result;
        }

        

        [SetUp]
        public void ResetTables()
        {
            //TODO : clear tables ?
            Setup.ClearAsc();
        }

        [Test]
        public void AddUser_IdentifyUser_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            long userId = AddUser_GetId_AssertSuccess(userName);
        }

       
        [Test]
        public void AddUser_UserNameTaken()
        {
            var userName = AssertAuxiliaries.NewName;
            long userId = AddUser_GetId_AssertSuccess(userName);
            var usp = new UserProvider();
            Response<Guid> ans = usp.AddUser(userName, Password);
            AssertAuxiliaries.ErrorAssert(FrontError.UserNameTaken,ans);
        }

        [Test]
        public void IdentifyUser_UserNameDoesNotExist()
        {
            const string inexistentUserName = "InexistentUserName";
            var usp = new UserProvider();
            var ans = usp.IdentifyUser(inexistentUserName, Password);
            AssertAuxiliaries.ErrorAssert(FrontError.UserNameDoesNotExist,ans);
        }

        [Test]
        public void IdentifyUser_WrongPassword()
        {
            var userName = AssertAuxiliaries.NewName;
            long userId = AddUser_GetId_AssertSuccess(userName);
            var usp = new UserProvider();
            Response<User> ans = usp.IdentifyUser(userName, "WrongPassword");
            AssertAuxiliaries.ErrorAssert(FrontError.WrongPassword,ans);
        }

        [Test]
        public void GetUser_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            long userId = AddUser_GetId_AssertSuccess(userName);
            var usp = new UserProvider();
            var ans = usp.GetUser(userId);
            Assert.IsTrue(ans.Success);
            Assert.IsNotNull(ans.Result);
            Assert.IsNull(ans.Error);
        }

        [Test]
        public void GetUser_IdDoesNotExist()
        {
            var usp = new UserProvider();
            var ans = usp.GetUser(4265468);
            AssertAuxiliaries.ErrorAssert(FrontError.UserIdDoesNotExist,ans);
        }

        


        [Test]
        public void SetPassword_Success()
        {
            var userName = AssertAuxiliaries.NewName;
            var id = AddUser_GetId_AssertSuccess(userName);
            const string newPassword = "NewPassword";
            User user = GetUser_AssertSuccess(id);
            Response<Guid> setPassword = user.SetPassword(Password, newPassword);
            Assert.IsTrue(setPassword.Success);
            System.Threading.Thread.Sleep(100);
            var usp = new UserProvider();
            Response<User> ans = usp.IdentifyUser(userName, newPassword);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void SetPassword_WrongPassword()
        {
            var userName = AssertAuxiliaries.NewName;
            var id = AddUser_GetId_AssertSuccess(userName);
            const string newPassword = "NewPassword";
            User user = GetUser_AssertSuccess(id);
            var setPassword = user.SetPassword("WrongPassword", newPassword);
            AssertAuxiliaries.ErrorAssert(FrontError.WrongPassword,setPassword);
        }

        [Test]
        public void SetPassword_InvalidString()
        {
            var userName = AssertAuxiliaries.NewName;
            var id = AddUser_GetId_AssertSuccess(userName);
            const string badString = "£££££"; //non ASCII
            User user = GetUser_AssertSuccess(id);
            var setPassword = user.SetPassword(Password, badString);
            AssertAuxiliaries.ErrorAssert(FrontError.InvalidString,setPassword);
        }

        

        
    }
}
