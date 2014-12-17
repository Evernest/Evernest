using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EvernestFront;
using EvernestBack;

namespace EvernestFrontTests
{
    class Setup
    {
        public static void ClearAsc()
        {
            var ascType = (typeof (AzureStorageClient));
            var field = ascType.GetField("_singleton", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            field.SetValue(null,null);
        }
    }
}
