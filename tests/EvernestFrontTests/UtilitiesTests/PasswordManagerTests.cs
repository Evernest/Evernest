using System;
using System.Collections.Generic;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;
using NUnit.Framework;

namespace EvernestFrontTests.UtilitiesTests
{
    [TestFixture]
    class PasswordManagerTests
    {
        [Test]
        public static void HashSaltVerify_Success()
        {
            var manager = new PasswordManager();
            var keyGenerator = new KeyGenerator();
            var password = keyGenerator.NewPassword();
            var hashSalt = manager.SaltAndHash(password);
            var verify = manager.Verify(password, hashSalt.Item1, hashSalt.Item2);
            Assert.IsTrue(verify);
        }

        [Test]
        public static void HashSaltVerify_ShortString_Success()
        {
            var manager = new PasswordManager();
            string password = "a";
            var hashSalt = manager.SaltAndHash(password);
            var verify = manager.Verify(password, hashSalt.Item1, hashSalt.Item2);
            Assert.IsTrue(verify);
        }

        [Test]
        public static void HashSaltVerify_WrongPassword()
        {
            var manager = new PasswordManager();
            var keyGenerator = new KeyGenerator();
            var password = keyGenerator.NewPassword();
            var notPassword = keyGenerator.NewPassword();
            var hashSalt = manager.SaltAndHash(password);
            var verify = manager.Verify(notPassword, hashSalt.Item1, hashSalt.Item2);
            Assert.IsFalse(verify);
        }

        [Test]
        public static void StringIsASCII_Success()
        {
            var manager = new PasswordManager();
            var keyGenerator = new KeyGenerator();
            var password = keyGenerator.NewPassword();
            var isASCII = manager.StringIsASCII(password);
            Assert.IsTrue(isASCII);
        }

        [Test]
        public static void StringIsASCII_ItIsNot()
        {
            var manager = new PasswordManager();
            var notASCII = "£";
            var isASCII = manager.StringIsASCII(notASCII);
            Assert.IsFalse(isASCII);
        }
    }
}
