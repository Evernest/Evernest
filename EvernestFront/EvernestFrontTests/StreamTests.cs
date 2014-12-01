using System;
using EvernestFront;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace EvernestFrontTests
{   
    [TestFixture]
    class StreamTests
    {
        private const string StreamName = "streamName";
        [Test]
        public void NewStream()
        {
            var stream = new Stream(StreamName);
            Assert.IsNotNull(stream);
        }
    }
}
