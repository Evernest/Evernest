using System.Collections.Generic;
using EvernestFront.Projections;
using EvernestFront.Service;

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

        public CommandHandler CommandHandler;
        public Dispatcher Dispatcher;
        public CommandResultManager CommandResultManager;

        private Injector() { }

        public void Build()
        {
            UsersProjection = new UsersProjection();
            EventStreamsProjection = new EventStreamsProjection();
            SourcesProjection = new SourcesProjection();
            CommandResultManager = new CommandResultManager();
            var systemEventStream = new SystemEventStream(0); //id for systemEventStream
            Dispatcher = new Dispatcher(new List<IProjection>
                {UsersProjection, EventStreamsProjection, SourcesProjection}, systemEventStream, CommandResultManager);

            var serviceData = new ServiceData(8); //8 eventStream ids and 8 user ids are reserved for system
            CommandHandler = new CommandHandler(serviceData, Dispatcher, CommandResultManager);
        }
    }
}
