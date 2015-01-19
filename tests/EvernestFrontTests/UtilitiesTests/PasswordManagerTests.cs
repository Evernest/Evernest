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
            var password = keyGenerator.NewKey();
            var tuple = manager.SaltAndHash(password);
            var verify = manager.Verify(password, tuple.Key, tuple.Value);
            Assert.IsTrue(verify);
        }

        [Test]
        public static void StringIsASCII_Success()
        {
            var manager = new PasswordManager();
            var keyGenerator = new KeyGenerator();
            var password = keyGenerator.NewKey();
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
