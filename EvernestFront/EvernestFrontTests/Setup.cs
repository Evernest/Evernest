using System.Reflection;
using EvernestBack;

namespace EvernestFrontTests
{
    class Setup
    {
        public static void ClearAsc()
        {
            //reflection magic to reset the AzureStorageClient between each test
            var ascType = (typeof (AzureStorageClient));
            var field = ascType.GetField("_singleton", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            field.SetValue(null,null);
        }
    }
}
