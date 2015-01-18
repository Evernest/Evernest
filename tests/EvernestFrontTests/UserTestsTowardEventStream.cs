﻿using NUnit.Framework;

namespace EvernestFrontTests
{

    [TestFixture]
    public class UserTestsTowardEventStream
    {
        private const string UserName = "userName";
        private const string UserName2 = "userName2";
        private const string StreamName = "streamName";
        private const string Message = "message";

        [SetUp]
        public void ResetTables()
        {
            //TODO : clear tables ?
            Setup.ClearAsc();
        }

        
    }
}
