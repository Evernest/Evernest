//using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using EvernestFront;
using EvernestFront.Contract;
using Assert = NUnit.Framework.Assert;

namespace EvernestFrontTests
{
    class Helpers
    {

        public static string NewName
        {
            get { return new EvernestFront.Utilities.KeyGenerator().NewKey(); }
        }

        internal static void ErrorAssert<T>(FrontError err,Response<T> ans)
        {
            Assert.IsFalse(ans.Success);
            Assert.IsNotNull(ans.Error);
            Assert.AreEqual(err, ans.Error);
        }


        

    }      
}
