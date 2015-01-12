using System.Web;
using System.Web.Optimization;

namespace EvernestAPI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Set to false for debug.
            BundleTable.EnableOptimizations = true;
        }
    }
}
