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
            //TODO: read system event stream
            injector.SystemCommandHandler.HandleCommands();
            injector.Dispatcher.DispatchSystemEvents();
        }
    }
}
