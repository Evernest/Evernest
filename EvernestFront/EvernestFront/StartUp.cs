using System;
using System.Diagnostics;
using EvernestBack;

namespace EvernestFront
{
    /// <summary>
    /// Execute setup operations to make the
    /// EvernestFront library operational. This class should be removed
    /// when Injector and AzureStorageClient are no longer singletons.
    /// Cf commentary for Injector.
    /// </summary>
    public class StartUp
    {
        public void Start()
        {
            var azureStorageClient = AzureStorageClient.Instance;
            var injector = Injector.Instance;
            injector.BuildFront(azureStorageClient);
        }

        
    }
}
