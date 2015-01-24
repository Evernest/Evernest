using System;
using System.Diagnostics;
using EvernestBack;

namespace EvernestFront
{
    public class StartUp
    {
        public void Start()
        {
            var azureStorageClient = AzureStorageClient.Instance;
            var injector = Injector.Instance;
            injector.BuildFront(azureStorageClient);
            injector.SystemEventRecuperator.FetchAndDispatch();
            injector.SystemCommandHandler.HandleCommands();
            injector.SystemEventQueue.DispatchSystemEvents();
            AzureStorageClient.Close();
        }
    }
}
