//using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using EvernestFront;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Responses;

namespace EvernestFrontTests
{
    class AssertAuxiliaries
    {

        private static int _counter = 0;

        public static string NewName
        {
            get
            {
                _counter = _counter + 1;
                return Convert.ToString(_counter);
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
