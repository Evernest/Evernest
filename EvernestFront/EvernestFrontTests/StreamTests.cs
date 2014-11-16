using System;
using EvernestFront;
using EvernestFront.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace EvernestFrontTests
{   
    [TestFixture]
    class StreamTests
    {
        private const string streamName = "streamName";
        [Test]
        public void NewStream()
        {
            var stream = new Stream(streamName);
            Assert.IsNotNull(stream);
        }
    }
}
