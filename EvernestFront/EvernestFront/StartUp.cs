using System;
using System.Diagnostics;
using EvernestBack;

namespace EvernestFront
{
    public class StartUp
    {
        public const string AzureStorageClientErrorMessage =
            "Error while getting AzureStorageClient.Instance in EvernestFront.StartUp.Start";

        public void Start()
        {
            object azureStorageClient;
            try
            {
                azureStorageClient = AzureStorageClient.Instance;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(AzureStorageClientErrorMessage);
                throw new Exception(AzureStorageClientErrorMessage, ex);
            }
            Debug.Assert(azureStorageClient is AzureStorageClient, "azureStorageClient is AzureStorageClient");
            var injector = Injector.Instance;
            injector.BuildFront((AzureStorageClient) azureStorageClient);
            //TODO: read system event stream
            injector.SystemCommandHandler.HandleCommands();
            injector.Dispatcher.DispatchSystemEvents();
        }
    }
}
