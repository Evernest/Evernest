//using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Diagnostics;
using EvernestFront;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Responses;

namespace EvernestFrontTests
{
    class AssertAuxiliaries
    {

        private static int counter = 0;

        public static string NewName
        {
            get
            {
                counter = counter + 1;
                return Convert.ToString(counter);
            }
        }

        internal static void ErrorAssert(FrontError err,BaseResponse ans)
        {
            Assert.IsFalse(ans.Success);
            Assert.IsNotNull(ans.Error);
            Assert.AreEqual(err, ans.Error);
        }


        

    }      
}
