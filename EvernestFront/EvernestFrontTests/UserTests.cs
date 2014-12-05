﻿using System;
using System.Collections.Generic;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvernestFront.Projection;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;
using EvernestFront.Errors;

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
            Projection.Clear();
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
            AssertAuxiliaries.ErrorAssert<UserNameTaken>(ans);
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
            AssertAuxiliaries.ErrorAssert<UserIdDoesNotExist>(ans);
        }

        [Test]
        public void IdentifyUser_WrongPassword()
        {
            AddUser addUser = User.AddUser(UserName);
            Assert.IsTrue(addUser.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, "BadPassword");
            AssertAuxiliaries.ErrorAssert<WrongPassword>(ans);
        }


        [Test]
        public void SetPassword_Success()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            const string newPassword = "NewPassword";
            User user = GetUser_AssertSuccess(userId);
            SetPassword setPassword = user.SetPassword(newPassword);
            Assert.IsTrue(setPassword.Success);
            IdentifyUser ans = User.IdentifyUser(UserName, newPassword);
            Assert.IsTrue(ans.Success);
        }

        [Test]
        public void SetPassword_InvalidString()
        {
            long userId = AddUser_GetId_AssertSuccess(UserName);
            const string badString = "£££££"; //non ASCII
            User user = GetUser_AssertSuccess(userId);
            SetPassword setPassword = user.SetPassword(badString);
            AssertAuxiliaries.ErrorAssert<InvalidString>(setPassword);
        }

        

        
    }
}
