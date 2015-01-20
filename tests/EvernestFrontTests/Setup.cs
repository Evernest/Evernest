using System;
using System.Reflection;
using EvernestBack;
using EvernestFront;
using NUnit.Framework;

namespace EvernestFrontTests
{
    [SetUpFixture]
    class Setup
    {

        public static void ClearAsc()
        {
            try
            {
                AzureStorageClient.Instance.ClearAll();
            }
            catch (Exception e)
            {
                Console.WriteLine("Sorry, I can't clear the blobs: "+e.ToString());
                Console.WriteLine("Magic workaround being used! (don't be too confident with the results)");
                //reflection magic to reset the AzureStorageClient between each test
                var ascType = (typeof(AzureStorageClient));
                var field = ascType.GetField("_singleton", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                field.SetValue(null, null);
            }
        }

        [SetUp]
        public static void RunBeforeAnyTests()
        {
            var startUp = new StartUp();
            startUp.Start();
        }
    }
}
