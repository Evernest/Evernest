using System.Collections.Generic;
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
        public Dispatcher Dispatcher;
        public SystemCommandResultManager SystemCommandResultManager;

        private Injector() { }

        public void Build()
        {
            UsersProjection = new UsersProjection();
            EventStreamsProjection = new EventStreamsProjection();
            SourcesProjection = new SourcesProjection();
            SystemCommandResultManager = new SystemCommandResultManager();
            var systemEventStream = new SystemEventStream(0); //id for systemEventStream
            Dispatcher = new Dispatcher(new List<IProjection>
                {UsersProjection, EventStreamsProjection, SourcesProjection}, systemEventStream, SystemCommandResultManager);
            var commandHandlingData = new SystemCommandHandlerState(8); //8 eventStream ids and 8 user ids are reserved for system
            SystemCommandHandler = new SystemCommandHandler(commandHandlingData, Dispatcher, SystemCommandResultManager);
            //TODO: read id for systemEventStream and number of reserved ids in app.config
        }
    }
}
