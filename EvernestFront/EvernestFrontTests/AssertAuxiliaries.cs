//using Microsoft.VisualStudio.TestTools.UnitTesting;

using EvernestFront;
using Assert = NUnit.Framework.Assert;
using EvernestFront.Answers;

namespace EvernestFrontTests
{
    class AssertAuxiliaries
    {
  


        internal static void ErrorAssert(FrontError err,Answer ans)
        {
            Assert.IsFalse(ans.Success);
            Assert.IsNotNull(ans.Error);
            Assert.AreEqual(err, ans.Error);
        }


        

    }      
}
