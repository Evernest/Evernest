using System.Collections.Generic;
using EvernestBack;
using EvernestFront.SystemCommandHandling;
using EvernestFront.Projections;

namespace EvernestFront
{
    class Injector
    {
// ReSharper disable once InconsistentNaming
        private static readonly Injector instance = new Injector();
        public static Injector Instance { get { return instance; } }

        public UsersProjection UsersProjection;
        public EventStreamsProjection EventStreamsProjection;
        public SourcesProjection SourcesProjection;

        public SystemCommandHandler SystemCommandHandler;
        public SystemEventQueue SystemEventQueue;
        public SystemCommandResultManager SystemCommandResultManager;
        public SystemEventRecuperator SystemEventRecuperator;

        private Injector() { }

        public void BuildFront(AzureStorageClient azureStorageClient)
        {
            UsersProjection = new UsersProjection();
            EventStreamsProjection = new EventStreamsProjection(azureStorageClient);
            SourcesProjection = new SourcesProjection();
            SystemCommandResultManager = new SystemCommandResultManager();
            var systemEventStream = new SystemEventStream(azureStorageClient, 0); //id for systemEventStream
            SystemEventQueue = new SystemEventQueue(new List<IProjection>
                {UsersProjection, EventStreamsProjection, SourcesProjection}, systemEventStream, SystemCommandResultManager);
            var systemCommandHandlerState = new SystemCommandHandlerState(8); //8 eventStream ids and 8 user ids are reserved for system
            SystemCommandHandler = new SystemCommandHandler(azureStorageClient, systemCommandHandlerState, SystemEventQueue, SystemCommandResultManager);
            SystemEventRecuperator = new SystemEventRecuperator(new List<IProjection>{UsersProjection, EventStreamsProjection, SourcesProjection, systemCommandHandlerState}, systemEventStream);
            //TODO: read id for systemEventStream and number of reserved ids in app.config
        }

        ~Injector()
        {
            AzureStorageClient.Close();
        }
    }
}
